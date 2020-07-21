using System;
using System.Collections.Generic;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.CommonWebApp.Ontraport;
using Xunit;

namespace HanumanInstitute.CommonWeb.Payments.Tests
{
    public partial class PaymentPlanProcessorTests
    {
        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_Due_ReturnsProduct(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    ProductName = ProductNameA,
                    TransactionsLeft = 2,
                    NextChargeDate = _now.AddDays(-1),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(plans[0].ProductName, result[0].Name);
            Assert.Equal(ProductPrice, result[0].Price);
            Assert.Equal(1, result[0].Quantity);
        }

        [Fact]
        public void GetBillProducts_NotDue_ReturnsEmpty()
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 2,
                    NextChargeDate = _now.AddDays(2),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_DueTwice_ReturnsProductWithQty2(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    ProductName = ProductNameA,
                    TransactionsLeft = 2,
                    NextChargeDate = _now.AddMonths(-1),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(plans[0].ProductName, result[0].Name);
            Assert.Equal(ProductPrice, result[0].Price);
            Assert.Equal(2, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_DueTriple_ReturnsProductWithQty3(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    ProductName = ProductNameA,
                    TransactionsLeft = 5,
                    NextChargeDate = _now.AddMonths(-2),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(plans[0].ProductName, result[0].Name);
            Assert.Equal(ProductPrice, result[0].Price);
            Assert.Equal(3, result[0].Quantity);
        }

        [Fact]
        public void GetBillProducts_TransactionsLeft_LimitQuantity()
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 2,
                    NextChargeDate = _now.AddMonths(-3),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now);

            Assert.Single(result);
            Assert.Equal(2, result[0].Quantity);
        }

        [Theory]
        [InlineData(-12)]
        [InlineData(0)]
        // [InlineData(12)]
        public void GetBillProducts_PeriodUnitDay_SetRightQuantity(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    NextChargeDate = _now.AddDays(-2),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Day
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(3, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PeriodUnitWeek_SetRightQuantity(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    NextChargeDate = _now.AddDays(-14),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Week
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(3, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PeriodUnitYear_SetRightQuantity(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    NextChargeDate = _now.AddYears(-2),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Year
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(3, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_Period10Days_SetRightQuantity(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    NextChargeDate = _now.AddDays(-10),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Day,
                    PeriodQty = 10
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(2, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_Period2Weeks_SetRightQuantity(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    NextChargeDate = _now.AddDays(-14),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Week,
                    PeriodQty = 2
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(2, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_Period3Months_SetRightQuantity(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    NextChargeDate = _now.AddMonths(-3),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Month,
                    PeriodQty = 3
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(2, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_Period2Years_SetRightQuantity(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    TransactionsLeft = 5,
                    NextChargeDate = _now.AddYears(-2),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.Year,
                    PeriodQty = 2
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(2, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PostpaidFullMonth_ReturnsProduct(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    ProductName = ProductNameA,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(plans[0].ProductName, result[0].Name);
            Assert.Equal(ProductPrice, result[0].Price);
            Assert.Equal(1, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PostpaidStartsOn_ReturnsLowerPrice(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    PaidUntilDate = NowMonth.AddDays(-28),
                    StartDate = NowMonth.AddDays(-15)
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.InRange(result[0].Price, ProductPrice * .4m, ProductPrice * .6m);
            Assert.Equal(1, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PostpaidLastPaymentOn_ReturnsLowerPrice(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    StartDate = NowMonth.AddDays(-28),
                    PaidUntilDate = NowMonth.AddDays(-15)
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.InRange(result[0].Price, ProductPrice * .4m, ProductPrice * .6m);
            Assert.Equal(1, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PostpaidEndsOn_ReturnsLowerPrice(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    EndDate = NowMonth.AddDays(-15)
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.InRange(result[0].Price, ProductPrice * .4m, ProductPrice * .6m);
            Assert.Equal(1, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PostpaidStartAndEnd_ReturnsLowerPrice(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    StartDate = _now.AddDays(-25),
                    EndDate = _now.AddDays(-10)
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.InRange(result[0].Price, ProductPrice * .4m, ProductPrice * .6m);
            Assert.Equal(1, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PostpaidDueTwoMonths_ReturnsDoublePrice(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    NextChargeDate = NowMonth.AddMonths(-1),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(ProductPrice * 2, result[0].Price * result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PostpaidDueTwoHalfMonths_ReturnsTwoHalfPrice(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    NextChargeDate = NowMonth.AddMonths(-1),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    PaidUntilDate = NowMonth.AddDays(-76)
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.InRange(result[0].Price, ProductPrice * 2.4m, ProductPrice * 2.6m);
            Assert.Equal(1, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_PostpaidDueThreeHalfMonths_ReturnsThreeHalfPrice(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    NextChargeDate = NowMonth.AddMonths(-2),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    StartDate = NowMonth.AddDays(-106)
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.InRange(result[0].Price, ProductPrice * 3.4m, ProductPrice * 3.6m);
            Assert.Equal(1, result[0].Quantity);
        }

        [Fact]
        public void GetBillProducts_PostpaidTrial_ReturnsEmpty()
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    //NextChargeDate = _now.AddDays(5),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial,
                    PeriodQty = 7,
                    StartDate = _now
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now);

            Assert.Empty(result);
        }

        [Fact]
        public void GetBillProducts_PostpaidTrialActive_ReturnsEmpty()
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial,
                    PeriodQty = 7,
                    StartDate = _now.AddDays(-3)
                }
            };

            var result = Model.GetBillProducts(CreateContact(paid: false), plans, _now);

            Assert.Empty(result);
        }

        [Fact]
        public void GetBillProducts_PostpaidTrialExpired_ReturnsEmpty()
        {
            // The initial payment must be done manually, it's not automatically added to the bill.
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    //NextChargeDate = _now,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial,
                    PeriodQty = 1,
                    StartDate = _now.AddDays(-15)
                }
            };

            var result = Model.GetBillProducts(CreateContact(paid: false), plans, _now);

            Assert.Empty(result);
        }

        [Fact]
        public void GetBillProducts_PostpaidTrialPlusPaid_ReturnsEmpty()
        {
            // Trial will be charged on the next billing cycle.
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    //NextChargeDate = _now,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial,
                    PeriodQty = 20,
                    StartDate = _now.AddDays(-15)
                },
                new ApiPaymentPlan()
                {
                    Id = 2,
                    NextChargeDate = NowMonth.AddMonths(1),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now);

            Assert.Empty(result);
        }

        [Fact]
        public void GetBillProducts_PostpaidTrialPlusMonth_BillMonthPlusTrial()
        {
            // Starting on the 15th, this should bill a full month + 20 more days of trial = ~45 days.
            var start = new DateTimeOffset(2020, 01, 15, 8, 0, 0, TimeSpan.Zero);
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial,
                    PeriodQty = 20,
                    StartDate = start
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, start.AddDays(50));

            Assert.Single(result);
            Assert.InRange(result[0].Price, ProductPrice * 1.40m, ProductPrice * 1.60m);
            Assert.Equal(1, result[0].Quantity);
        }

        [Fact]
        public void GetBillProducts_PaidTrialCancel_ReturnsEmpty()
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial,
                    PeriodQty = 20,
                    StartDate = _now.AddDays(-15),
                    EndDate = _now
                }
            };

            var result = Model.GetBillProducts(CreateContact(paid: true), plans, _now);

            Assert.Empty(result);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_TwoProductsDue_ReturnsTwoProducts(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    ProductName = ProductNameA,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                },
                new ApiPaymentPlan()
                {
                    Id = 2,
                    ProductName = ProductNameB,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Equal(2, result.Count);
            Assert.Equal(ProductNameA, result[0].Name);
            Assert.Equal(ProductPrice, result[0].Price);
            Assert.Equal(1, result[0].Quantity);
            Assert.Equal(ProductNameB, result[1].Name);
            Assert.Equal(ProductPrice, result[1].Price);
            Assert.Equal(1, result[1].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_TwoSameProductsDue_ReturnsProductQuantity2(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    ProductName = ProductNameA,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                },
                new ApiPaymentPlan()
                {
                    Id = 2,
                    ProductName = ProductNameA,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(ProductNameA, result[0].Name);
            Assert.Equal(ProductPrice, result[0].Price);
            Assert.Equal(2, result[0].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_TwoSameProductsDueDiffAmount_ReturnsTwoProducts(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    ProductName = ProductNameA,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                },
                new ApiPaymentPlan()
                {
                    Id = 2,
                    ProductName = ProductNameA,
                    NextChargeDate = NowMonth,
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid,
                    StartDate = NowMonth.AddDays(-15)
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Equal(2, result.Count);
            Assert.Equal(ProductNameA, result[0].Name);
            Assert.Equal(ProductPrice, result[0].Price);
            Assert.Equal(1, result[0].Quantity);
            Assert.Equal(ProductNameA, result[1].Name);
            Assert.InRange(result[1].Price, ProductPrice * .4m, ProductPrice * .6m);
            Assert.Equal(1, result[1].Quantity);
        }

        [Theory, MemberData(nameof(DiffHours))]
        public void GetBillProducts_TwoSameProductsDue2Months_ReturnsProductQuantity4(int addHours)
        {
            var plans = new List<ApiPaymentPlan>()
            {
                new ApiPaymentPlan()
                {
                    Id = 1,
                    ProductName = ProductNameA,
                    NextChargeDate = NowMonth.AddMonths(-1),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaid
                },
                new ApiPaymentPlan()
                {
                    Id = 2,
                    ProductName = ProductNameA,
                    NextChargeDate = NowMonth.AddMonths(-1),
                    PricePerPeriod = ProductPrice,
                    PeriodUnit = PaymentPeriodUnit.MonthPostpaidTrial
                }
            };

            var result = Model.GetBillProducts(CreateContact(), plans, _now.AddHours(addHours));

            Assert.Single(result);
            Assert.Equal(ProductNameA, result[0].Name);
            Assert.Equal(ProductPrice, result[0].Price);
            Assert.Equal(4, result[0].Quantity);
        }
    }
}
