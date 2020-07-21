using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.CommonWebApp.Ontraport;
using Moq;
using NodaTime;
using NodaTime.Extensions;
using Xunit;

namespace HanumanInstitute.CommonWeb.Payments.Tests
{
    public partial class PaymentPlanProcessorTests
    {
        private List<ApiPaymentPlan> CreatePlanProcess(bool due = true, decimal? price = null)
        {
            return new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 2,
                    NextChargeDate = due ? _now : _now.AddDays(2),
                    PricePerPeriod = price ?? ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };
        }

        private void SetProcessingResult(PaymentStatus status)
        {
            MockProcessor.Setup(x => x.SubmitAsync(It.IsAny<ProcessOrder>()))
                .Returns(Task.FromResult(new PaymentResult(status) { TransactionId = "888" }));
        }

        private static DateTimeOffset GetMonthStartDate(DateTimeOffset planDate) => new DateTimeOffset(planDate.Year, planDate.Month, 1, 0, 0, 0, TimeSpan.Zero);

        [Fact]
        public async Task ProcessAsync_NoProductDue_SuccessNoProcess()
        {
            var contact = CreateContact();
            var plans = CreatePlanProcess(false);

            var result = await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(PaymentStatus.Approved, result.Status);
            MockProcessor.Verify(x => x.SubmitAsync(It.IsAny<ProcessOrder>()), Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_PayWithCredits_SuccessNoProcess()
        {
            var contact = CreateContact(credits: ProductPrice);
            var plans = CreatePlanProcess(true);

            var result = await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(PaymentStatus.Approved, result.Status);
            MockProcessor.Verify(x => x.SubmitAsync(It.IsAny<ProcessOrder>()), Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_PayWithCredits_ReduceCredits()
        {
            var extraCredits = 55.55m;
            var contact = CreateContact(credits: ProductPrice + extraCredits);
            var plans = CreatePlanProcess(true);

            await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(extraCredits, contact.AccountCredits);
            MockContacts.Verify(x => x.UpdateAsync(contact.Id!.Value, It.IsAny<object?>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_InsufficientCredits_ReturnsDeclined()
        {
            var contact = CreateContact(paymentMethod: PaymentMethod.None);
            var plans = CreatePlanProcess(true);

            var result = await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(PaymentStatus.Declined, result.Status);
        }

        [Fact]
        public async Task ProcessAsync_PartialPayWithCredits_ReduceCreditsAndProcess()
        {
            var contact = CreateContact(credits: 20);
            var plans = CreatePlanProcess(true, 100);
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(0, contact.AccountCredits);
            MockProcessor.Verify(x => x.SubmitAsync(It.IsAny<ProcessOrder>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_PartialPayWithCreditsDeclined_DoNotReduceCredits()
        {
            var contact = CreateContact(credits: 20);
            var plans = CreatePlanProcess(true, 100);
            SetProcessingResult(PaymentStatus.Declined);

            await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(20, contact.AccountCredits);
            MockProcessor.Verify(x => x.SubmitAsync(It.IsAny<ProcessOrder>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_PayWithPayPal_ReturnApproved()
        {
            var contact = CreateContact(paymentMethod: PaymentMethod.PayPal);
            var plans = CreatePlanProcess(true, 100);
            SetProcessingResult(PaymentStatus.Approved);

            var result = await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(PaymentStatus.Approved, result.Status);
            MockProcessor.Verify(x => x.SubmitAsync(It.IsAny<ProcessOrder>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_NoPaymentMethod_ReturnsDeclined()
        {
            var contact = CreateContact(paymentMethod: PaymentMethod.None);
            var plans = CreatePlanProcess(true, 100);

            var result = await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(PaymentStatus.Declined, result.Status);
            MockProcessor.Verify(x => x.SubmitAsync(It.IsAny<ProcessOrder>()), Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_Valid_HasPaidHealingTechnologiesTrue()
        {
            var contact = CreateContact();
            var plans = CreatePlanProcess(true);
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(true, contact.HasPaidHealingTechnologies);
        }

        [Fact]
        public async Task ProcessAsync_Declined_HasPaidHealingTechnologiesUnchanged()
        {
            var contact = CreateContact();
            var plans = CreatePlanProcess(true);
            SetProcessingResult(PaymentStatus.Declined);
            var hasPaid = contact.HasPaidHealingTechnologies;

            await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(hasPaid, contact.HasPaidHealingTechnologies);
        }


        [Fact]
        public async Task ProcessAsync_Valid_CollectionPlanIdReset()
        {
            var contact = CreateContact();
            var plans = CreatePlanProcess(true);
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now);

            Assert.Null(contact.CollectionPlanId);
        }

        [Fact]
        public async Task ProcessAsync_Valid_RecurringLastMasterIdChanged()
        {
            var contact = CreateContact();
            var plans = CreatePlanProcess(true);
            SetProcessingResult(PaymentStatus.Approved);
            var masterId = contact.RecurringLastMasterId;

            await Model.ProcessAsync(contact, plans, _now);

            Assert.NotEqual(masterId, contact.RecurringLastMasterId);
        }

        [Fact]
        public async Task ProcessAsync_Valid_LastAttemptDateNow()
        {
            var contact = CreateContact();
            var plans = CreatePlanProcess(true);
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(_now, plans[0].LastAttemptDate);
        }

        [Fact]
        public async Task ProcessAsync_Declined_LastAttemptDateNow()
        {
            var contact = CreateContact();
            var plans = CreatePlanProcess(true);
            SetProcessingResult(PaymentStatus.Declined);

            await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(_now, plans[0].LastAttemptDate);
            MockPlans.Verify(x => x.UpdateAsync(plans[0].Id!.Value, It.IsAny<object?>()), Times.Once);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPlan_SetPaidUntilDate(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 2,
                    PaidUntilDate = _now.AddMonths(-1),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(_now, plans[0].PaidUntilDate);
            Assert.Equal(_now.AddMonths(1), plans[0].NextChargeDate);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_10DayPlan_SetPaidUntilDate(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 2,
                    PaidUntilDate = _now.AddDays(-10),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Day,
                    PeriodQty = 10
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(_now, plans[0].PaidUntilDate);
            Assert.Equal(_now.AddDays(10), plans[0].NextChargeDate);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_2YearPlanPay2Periods_SetPaidUntilDate(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 2,
                    PaidUntilDate = _now.AddYears(-4),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Year,
                    PeriodQty = 2
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(_now, plans[0].PaidUntilDate);
            Assert.Equal(_now.AddYears(2), plans[0].NextChargeDate);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPostpaidPlan_SetPaidUntilDate(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    StartDate = _now.AddDays(-20),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            var monthStart = GetMonthStartDate(_now);
            Assert.Equal(monthStart, plans[0].PaidUntilDate);
            Assert.InRange(plans[0].NextChargeDate!.Value, monthStart.AddMonths(1), monthStart.AddMonths(1).AddDays(1));
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPostpaidPlan_SetNextChargeLocalTime(int addHours)
        {
            var contact = CreateContact();
            contact.Timezone = "America/Los_Angeles";
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    StartDate = _now.AddDays(-20),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            // Time should be set between 8am and 10am local time.
            var zone = DateTimeZoneProviders.Tzdb[contact.Timezone!];
            var localDate = Instant.FromDateTimeOffset(plans[0].NextChargeDate!.Value).InZone(zone);
            Assert.InRange(localDate.TimeOfDay, LocalTime.FromHoursSinceMidnight(8), LocalTime.FromHoursSinceMidnight(10));
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPostpaidPayTwoMonths_SetPaidUntilDate(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    StartDate = _now.AddDays(-50),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            var monthStart = GetMonthStartDate(_now);
            Assert.Equal(monthStart, plans[0].PaidUntilDate);
            Assert.InRange(plans[0].NextChargeDate!.Value, monthStart.AddMonths(1), monthStart.AddMonths(1).AddDays(1));
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPostpaidTrialActive_ReturnsApproved(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    StartDate = _now.AddDays(-3),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            var result = await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(PaymentStatus.Approved, result.Status);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPostpaidTrialActive_NoChange(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    StartDate = _now.AddDays(-3),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial
                }
            };
            plans.ForEach(x => x.ClearChanges());
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            MockProcessor.Verify(x => x.SubmitAsync(It.IsAny<ProcessOrder>()), Times.Never);
            MockContacts.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<object?>()), Times.Never);
            MockPlans.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<object?>()), Times.Never);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPostpaidTrialExpired_SetPaidUntilDate(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    StartDate = _now.AddDays(-30),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            var monthStart = GetMonthStartDate(_now);
            Assert.Equal(monthStart, plans[0].PaidUntilDate);
            Assert.InRange(plans[0].NextChargeDate!.Value, monthStart.AddMonths(1), monthStart.AddMonths(1).AddDays(1));
        }

        [Fact]
        public async Task ProcessAsync_MonthPostpaidTrialPaid_SetPeriodUnit()
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    StartDate = _now.AddDays(-30),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now);

            Assert.Equal(PaymentPeriodUnit.MonthPostpaid, plans[0].PeriodUnit);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPostpaidTrialNotRenewed_SetEndDate(int addHours)
        {
            var contact = CreateContact(paid: false);
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    StartDate = _now.AddDays(-7),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(_now.AddHours(addHours), plans[0].EndDate);
            MockProcessor.Verify(x => x.SubmitAsync(It.IsAny<ProcessOrder>()), Times.Never);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_WeekPlan_ReduceTransactionsLeft(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 2,
                    StartDate = _now.AddDays(-10),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Week
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(1, plans[0].TransactionsLeft);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_PayTwoMonths_ReduceTransactionsLeftByTwo(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    StartDate = _now.AddDays(-70),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(3, plans[0].TransactionsLeft);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_SingleTransactionLeftTryPayTwo_TransactionLeftZero(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 1,
                    StartDate = _now.AddDays(-70),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(0, plans[0].TransactionsLeft);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPlan_IncreaseTotalCharged(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    StartDate = _now.AddDays(-50),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(ProductPrice, plans[0].TotalCharged);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_MonthPrepaidTrialPlan_IncreaseTotalCharged(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    StartDate = _now.AddDays(-50),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial,
                    TotalCharged = 50
                }
            };
            SetProcessingResult(PaymentStatus.Approved);
            var total = Model.GetBillProducts(contact, plans, _now.AddHours(addHours)).CalculateTotal();

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(total + 50, plans[0].TotalCharged);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_Suspended_SetStartDateClearEndDate(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    StartDate = _now.AddDays(-50),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    EndDate = _now.AddDays(-10)
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            var monthStart = GetMonthStartDate(_now);
            Assert.Null(plans[0].EndDate);
            Assert.Equal(_now.AddHours(addHours), plans[0].StartDate);
            Assert.InRange(plans[0].NextChargeDate!.Value, monthStart.AddMonths(1), monthStart.AddMonths(1).AddDays(1));
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_DeletedEndDateBilled_Remove(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    StartDate = _now.AddDays(-50),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    EndDate = _now.AddDays(-15),
                    Deleted = true
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            MockPlans.Verify(x => x.DeleteAsync(plans[0].Id!.Value), Times.Once);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_DeletedEndDateNotBilled_DoNotRemove(int addHours)
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    StartDate = _now.AddDays(-50),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    EndDate = _now.AddDays(-1),
                    Deleted = true
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            MockPlans.Verify(x => x.DeleteAsync(plans[0].Id!.Value), Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_PaymentPlanCompleted_Remove()
        {
            var contact = CreateContact();
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 1,
                    NextChargeDate = _now,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };
            SetProcessingResult(PaymentStatus.Approved);

            await Model.ProcessAsync(contact, plans, _now);

            MockPlans.Verify(x => x.DeleteAsync(plans[0].Id!.Value), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_PaymentFailsWithinGracePeriod_DoNotSetEndDate()
        {
            var contact = CreateContact(paymentMethod: PaymentMethod.None);
            var monthStart = GetMonthStartDate(_now);
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 1,
                    NextChargeDate = monthStart,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                }
            };

            await Model.ProcessAsync(contact, plans, monthStart.AddDays(4));

            Assert.Null(plans[0].EndDate);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public async Task ProcessAsync_PaymentFails_SetEndDate(int addHours)
        {
            var contact = CreateContact(paymentMethod: PaymentMethod.None);
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 1,
                    NextChargeDate = _now.AddDays(-5), // Grace period is 5 days by default
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                }
            };

            await Model.ProcessAsync(contact, plans, _now.AddHours(addHours));

            Assert.Equal(_now.AddHours(addHours), plans[0].EndDate);
        }
    }
}
