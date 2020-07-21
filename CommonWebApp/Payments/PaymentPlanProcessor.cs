using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.CommonWeb.Payments;
using HanumanInstitute.CommonWeb.Validation;
using HanumanInstitute.CommonWebApp.Ontraport;
using HanumanInstitute.OntraportApi.Models;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.TimeZones;
using Res = HanumanInstitute.CommonWebApp.Properties.Resources;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Manages the processing of recurring payments.
    /// </summary>
    public class PaymentPlanProcessor : IPaymentPlanProcessor, IDisposable
    {
        private readonly IPaymentProcessor _processor;
        private readonly IOntraportContacts _ontraContacts;
        private readonly IOntraportPaymentPlans _ontraPlans;
        private readonly IRandomGenerator _random;
        private readonly IOptions<PaymentProcessorConfig> _config;

        private readonly SemaphoreSlim _queue = new SemaphoreSlim(1);

        public PaymentPlanProcessor(IPaymentProcessor processor, IOntraportContacts ontraContacts, IOntraportPaymentPlans ontraPlans, IRandomGenerator random, IOptions<PaymentProcessorConfig> config)
        {
            _processor = processor;
            _ontraContacts = ontraContacts;
            _ontraPlans = ontraPlans;
            _random = random;
            _config = config;
        }

        /// <summary>
        /// Returns the Ontraport data for specified contact.
        /// </summary>
        /// <param name="contactId">The contact to retrieve information for.</param>
        /// <returns>The contact information.</returns>
        public async Task<ApiCustomContact> GetContactAsync(int contactId) =>
            await _ontraContacts.SelectAsync(contactId).ConfigureAwait(false);

        /// <summary>
        /// Returns all recurring payment plans for specified contact.
        /// </summary>
        /// <param name="contactId">The contact to retrieve payment plans for.</param>
        /// <returns>The list of payment plans.</returns>
        public async Task<IEnumerable<ApiPaymentPlan>> GetPaymentPlansAsync(int contactId)
        {
            return await _ontraPlans.SelectAsync(new ApiSearchOptions()
                .AddCondition(ApiPaymentPlan.ContactIdKey, "=", contactId)
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the list of products to settle due payments.
        /// </summary>
        /// <param name="contact">The contact to create the bill for.</param>
        /// <param name="planList">The list of due payment plans.</param>
        /// <param name="currentDate">The current date, ensuring all calculations are done using the exact same date.</param>
        /// <param name="deduct">An amount to deduct from the order, such as account credits.</param>
        /// <returns>The list of products to pay.</returns>
        public IList<ProcessOrderProduct> GetBillProducts(ApiCustomContact contact, IEnumerable<ApiPaymentPlan> planList, DateTimeOffset currentDate)
        {
            contact.CheckNotNull(nameof(contact));
            planList.CheckNotNull(nameof(planList));

            var products = new List<ProcessOrderProduct>();
            // Don't bill trials until something has been paid.
            foreach (var plan in planList.Where(x => contact.HasPaidHealingTechnologies == true || x.PeriodUnit != PaymentPeriodUnit.MonthPostpaidTrial))
            {
                //var isTrialExpired = ((currentDate - (plan.StartDate ?? plan.DateAdded!.Value)).TotalDays > (plan.PeriodQty ?? _config.Value.DefaultTrialDays))
                var qty = CalcPeriodQty(plan, currentDate);

                if (qty > 0)
                {
                    // Apply TransactionsLeft.
                    //var maxQty = plan.TransactionsLeft ?? (plan.PeriodUnit == PaymentPeriodUnit.MonthPostpaid || plan.PeriodUnit == PaymentPeriodUnit.MonthPostpaidTrial ? (int?)null : 1);
                    //if (maxQty != null && qty > maxQty)
                    //{
                    //    qty = maxQty.Value;
                    //}

                    // Add product to cart.
                    var product = new ProcessOrderProduct(plan.ProductName ?? string.Empty);
                    if ((qty % 1) == 0)
                    {
                        product.Quantity = (int)qty;
                        product.Price = plan.PricePerPeriod ?? 0;
                    }
                    else
                    {
                        product.Quantity = 1;
                        product.Price = Math.Round((plan.PricePerPeriod ?? 0) * qty, 2);
                    }
                    products.Add(product);
                }
            }

            return products.GroupDuplicates();
        }

        /// <summary>
        /// Returns how many plan periods are due.
        /// </summary>
        /// <param name="plan">The payment plan to calculate.</param>
        /// <param name="currentDate">The current date, ensuring all calculations are done using the exact same date.</param>
        /// <returns>The amount of periods to pay.</returns>
        private decimal CalcPeriodQty(ApiPaymentPlan plan, DateTimeOffset currentDate)
        {
            var loopDate = new[] { plan.StartDate, plan.PaidUntilDate, plan.DateAdded }.Max() ?? MovePeriodDate(plan, plan.NextChargeDate!.Value, -1);
            var qtyTotal = 0m;
            var qtyTrial = 0m;
            var count = 0;
            while (GetPlanPeriod(plan, loopDate, currentDate, out var periodStart, out var periodEnd) &&
                (plan.TransactionsLeft == null || count < plan.TransactionsLeft))
            {
                count++;
                var fullPeriod = periodEnd - periodStart;
                var billPeriod = periodEnd - loopDate;
                if (plan.EndDate.HasValue)
                {
                    billPeriod -= (periodEnd - plan.EndDate.Value);
                }
                // Calculate period by period.
                var qty = (decimal)billPeriod.Ticks / fullPeriod.Ticks;

                // Trial period will only be billed once we've completed a billing cycle afterwards.
                if (plan.PeriodUnit == PaymentPeriodUnit.MonthPostpaidTrial && billPeriod.TotalDays <= (plan.PeriodQty ?? _config.Value.DefaultTrialDays))
                {
                    qtyTrial += qty;
                }
                else
                {
                    qtyTotal += qty + qtyTrial;
                    qtyTrial = 0;
                }

                loopDate = periodEnd;
            }
            return qtyTotal;
        }

        /// <summary>
        /// Calculates the start and end dates of a period, and returns whether the period end is past and due to be paid.
        /// </summary>
        /// <param name="plan">The payment plan to calculate.</param>
        /// <param name="planDate">The date to calculate period start and end dates for.</param>
        /// <param name="currentDate">The current date, ensuring all calculations are done using the exact same date.</param>
        /// <param name="periodStart">Returns the period start date.</param>
        /// <param name="periodEnd">Returns the period end date.</param>
        /// <returns>Whether endDate is past and due to be paid.</returns>
        private bool GetPlanPeriod(ApiPaymentPlan plan, DateTimeOffset planDate, DateTimeOffset currentDate, out DateTimeOffset periodStart, out DateTimeOffset periodEnd)
        {
            if (plan.PeriodUnit == PaymentPeriodUnit.MonthPostpaid || plan.PeriodUnit == PaymentPeriodUnit.MonthPostpaidTrial)
            {
                periodStart = new DateTimeOffset(planDate.Year, planDate.Month, 1, 0, 0, 0, TimeSpan.Zero);
            }
            else
            {
                periodStart = planDate;
            }
            periodEnd = MovePeriodDate(plan, periodStart, 1);
            return periodEnd <= currentDate.AddHours(_config.Value.PayHoursAhead);
        }

        /// <summary>
        /// Moves the date by specified number of full billing periods.
        /// </summary>
        /// <param name="plan">The plan containing billing period info.</param>
        /// <param name="datePlan">The date to move.</param>
        /// <param name="qty">The amount of full periods to add or remove to the date.</param>
        /// <returns>The date for a different billing period.</returns>
        private static DateTimeOffset MovePeriodDate(ApiPaymentPlan plan, DateTimeOffset datePlan, int qty)
        {
            return plan.PeriodUnit switch
            {
                PaymentPeriodUnit.MonthPostpaid => datePlan.AddMonths(1 * qty),
                PaymentPeriodUnit.MonthPostpaidTrial => datePlan.AddMonths(1 * qty),
                PaymentPeriodUnit.Day => datePlan.AddDays((plan.PeriodQty ?? 1) * qty),
                PaymentPeriodUnit.Week => datePlan.AddDays(7 * (plan.PeriodQty ?? 1) * qty),
                PaymentPeriodUnit.Month => datePlan.AddMonths((plan.PeriodQty ?? 1) * qty),
                PaymentPeriodUnit.Year => datePlan.AddYears((plan.PeriodQty ?? 1) * qty),
                _ => throw new InvalidOperationException(Res.GetPlanPeriodUnrecognizedUnit)
            };
        }

        /// <summary>
        /// When in collection, disables the Ontraport Collection campaign for all due payment plans except the Collection Plan ID, to avoid double-processing. Returns whether this request should be processed.
        /// </summary>
        /// <param name="contact">The contact to process payment for.</param>
        /// <param name="planList">The list of payment plans that the contact has.</param>
        /// <param name="callingPlanId">The id of the Ontraport payment plan that is calling this function via WebHook.</param>
        /// <returns>Whether this request should be processed.</returns>
        public async Task<bool> ResetCollectionAsync(ApiCustomContact contact, IEnumerable<ApiPaymentPlan> planList, int callingPlanId)
        {
            contact.CheckNotNull(nameof(contact));
            planList.CheckNotNull(nameof(planList));
            contact.Id.CheckNotNull(nameof(contact.Id));

            // If we're in collection, any payment plan that is not the one managing the collection sequence is deactivated to avoid double-processing when multiple payments fail.
            if (contact.CollectionPlanId.HasValue)
            {
                foreach (var item in planList)
                {
                    if (item.Id.HasValue && item.Id != contact.CollectionPlanId && item.ListCampaigns.Contains(_config.Value.PaymentPlanCampaignId))
                    {
                        await _ontraPlans.RemoveFromCampaignAsync(item.Id.Value, _config.Value.PaymentPlanCampaignId).ConfigureAwait(false);
                    }
                }
            }

            return contact.CollectionPlanId == null || contact.CollectionPlanId == callingPlanId;
        }


        /// <summary>
        /// Processes the payment for specified payment plans.
        /// </summary>
        /// <param name="contact">The contact to process payments for.</param>
        /// <param name="planList">The payment plans to process.</param>
        /// <param name="currentDate">The current date, ensuring all calculations are done using the exact same date.</param>
        /// <returns>The result of the transaction.</returns>
        public virtual async Task<PaymentResult> ProcessAsync(ApiCustomContact contact, IEnumerable<ApiPaymentPlan> planList, DateTimeOffset currentDate)
        {
            if (contact == null) { throw new ArgumentNullException(nameof(contact)); }
            if (planList == null) { throw new ArgumentNullException(nameof(planList)); }
            //contact.CheckNotNull(nameof(contact));  // FxCop not working with this?
            //planList.CheckNotNull(nameof(planList));

            PaymentResult result = new PaymentResult(PaymentStatus.Approved);
            var products = GetBillProducts(contact, planList, currentDate);

            // Try to pay using account credits.
            var creditLeft = products.ApplyDiscount(contact.AccountCredits ?? 0);
            var payTotal = products.CalculateTotal();

            if (payTotal > 0 && (contact.RecurringPaymentMethod == PaymentMethod.CreditCard || contact.RecurringPaymentMethod == PaymentMethod.PayPal))
            {
                var order = new ProcessOrder()
                {
                    Address = new ProcessOrderAddress()
                    {
                        FirstName = contact.FirstName ?? string.Empty,
                        LastName = contact.LastName ?? string.Empty,
                        Address = contact.Address ?? string.Empty,
                        Address2 = contact.Address2,
                        City = contact.City ?? string.Empty,
                        State = contact.State ?? string.Empty,
                        PostalCode = contact.Zip ?? string.Empty,
                        Country = contact.Country ?? string.Empty,
                        Phone = contact.HomePhone ?? string.Empty,
                        Email = contact.Email ?? string.Empty
                    }
                }.SetProducts(products);

                if (contact.RecurringPaymentMethod == PaymentMethod.CreditCard)
                {
                    order.CreditCardMasterId = contact.RecurringLastMasterId;
                }

                // Process transaction.
                result = await _processor.SubmitAsync(order).ConfigureAwait(false);
                if (result.Status == PaymentStatus.Approved)
                {
                    int? transId = null;
                    if (int.TryParse(result.TransactionId, out var outId))
                    {
                        transId = outId;
                    }
                    contact.RecurringLastMasterId = transId;
                }
            }
            else if (payTotal > 0)
            {
                result = new PaymentResult(PaymentStatus.Declined, Res.PaymentProcessInsufficientCredits);
            }
            else // Pay with account credits
            {
                result = new PaymentResult(PaymentStatus.Approved);
            }

            if (result.Status == PaymentStatus.Approved)
            {
                if (contact.AccountCredits != creditLeft)
                {
                    contact.AccountCredits = creditLeft;
                }
                if (contact.CollectionPlanId.HasValue)
                {
                    contact.CollectionPlanId = null;
                }
            }

            UpdatePlanInfo(contact, planList, currentDate, result.Status);

            await SaveChangesAsync(contact, planList).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// If status is Approved, marks all due payments as paid, otherwise set Last Attempt Date.
        /// </summary>
        private void UpdatePlanInfo(ApiCustomContact contact, IEnumerable<ApiPaymentPlan> planList, DateTimeOffset currentDate, PaymentStatus status)
        {
            foreach (var plan in planList)
            {
                if (contact.HasPaidHealingTechnologies == false && plan.PeriodUnit == PaymentPeriodUnit.MonthPostpaidTrial)
                {
                    // End trial without billing.
                    plan.EndDate = currentDate;
                }
                else
                {
                    UpdatePlanInfo(contact, plan, currentDate, status);
                }
            }
        }

        /// <summary>
        /// If status is Approved, mark payment plan as paid, otherwise set Last Attempt Date. No change if payment plan is not due.
        /// </summary>
        private void UpdatePlanInfo(ApiCustomContact contact, ApiPaymentPlan plan, DateTimeOffset currentDate, PaymentStatus status)
        {
            var loopDate = new[] { plan.StartDate, plan.PaidUntilDate, plan.DateAdded }.Max() ?? MovePeriodDate(plan, plan.NextChargeDate!.Value, -1);
            var qtyTotal = 0m;
            var qtyTrial = 0m;
            var count = 0;
            DateTimeOffset? paidUntil = null;

            while (GetPlanPeriod(plan, loopDate, currentDate, out var periodStart, out var periodEnd) &&
                (plan.TransactionsLeft == null || count < plan.TransactionsLeft))
            {
                count++;
                var fullPeriod = periodEnd - periodStart;
                var billPeriod = periodEnd - loopDate;
                if (plan.EndDate.HasValue)
                {
                    billPeriod -= (periodEnd - plan.EndDate.Value);
                }
                // Calculate period by period.
                var qty = (decimal)billPeriod.Ticks / fullPeriod.Ticks;

                // Trial period will only be billed once we've completed a billing cycle afterwards.
                if (plan.PeriodUnit == PaymentPeriodUnit.MonthPostpaidTrial && billPeriod.TotalDays <= (plan.PeriodQty ?? _config.Value.DefaultTrialDays))
                {
                    qtyTrial += qty;
                }
                else
                {
                    qtyTotal += qty + qtyTrial;
                    qtyTrial = 0;
                }
                paidUntil = periodEnd;

                loopDate = periodEnd;
            }

            if (qtyTotal > 0)
            {
                plan.LastAttemptDate = currentDate;

                if (status == PaymentStatus.Approved)
                {
                    if (plan.TransactionsLeft.HasValue)
                    {
                        plan.TransactionsLeft -= count;
                    }
                    plan.PaidUntilDate = paidUntil;
                    plan.NextChargeDate = MovePeriodDate(plan, paidUntil!.Value, 1);
                    plan.TotalCharged = Math.Round((plan.TotalCharged ?? 0) + qtyTotal * plan.PricePerPeriod!.Value, 2);

                    if (plan.PeriodUnit == PaymentPeriodUnit.MonthPostpaidTrial)
                    {
                        plan.PeriodUnit = PaymentPeriodUnit.MonthPostpaid;
                    }
                    if (plan.PeriodUnit == PaymentPeriodUnit.MonthPostpaid)
                    {
                        plan.NextChargeDate = SetNextChargeTime(contact, plan.NextChargeDate!.Value);
                    }

                    // Resume suspended subscription.
                    if (plan.EndDate.HasValue && plan.Deleted != true)
                    {
                        plan.StartDate = currentDate;
                        plan.EndDate = null;
                    }
                }
                else if (status == PaymentStatus.Declined && (currentDate - paidUntil!.Value).TotalDays >= _config.Value.PaymentFaultDisableAfterDays)
                {
                    // Suspend account for unpaid bill.
                    plan.EndDate = currentDate;
                }
            }
        }

        /// <summary>
        /// Sets NextChargeDate to be between 8am and 10am in local time zone. This will avoid all recurring payments to be processed at the exact same time.
        /// </summary>
        private DateTimeOffset SetNextChargeTime(ApiCustomContact contact, DateTimeOffset planDate)
        {
            ZonedDateTime dayStart = ZonedDateTime.FromDateTimeOffset(planDate);
            try
            {
                var zone = DateTimeZoneProviders.Tzdb[contact.Timezone ?? ""];
                dayStart = zone.AtStartOfDay(LocalDate.FromDateTime(planDate.UtcDateTime));
            }
            catch (DateTimeZoneNotFoundException) { }

            var localTime = dayStart + Duration.FromHours(8) + Duration.FromMinutes(_random.GetInt(120));
            return localTime.ToDateTimeOffset();
        }

        /// <summary>
        /// Saves all the changes back to Ontraport.
        /// </summary>
        private async Task SaveChangesAsync(ApiCustomContact contact, IEnumerable<ApiPaymentPlan> planList)
        {
            var taskList = new List<Task>();

            var changes = contact.GetChanges();
            if (changes.Any())
            {
                taskList.Add(_ontraContacts.UpdateAsync(contact.Id!.Value, contact.GetChanges()));
            }
            foreach (var plan in planList)
            {
                // Remove delete subscriptions that are paid, and plans that are fully paid.
                if (plan.TransactionsLeft == 0 || (plan.Deleted == true && plan.PaidUntilDate >= plan.EndDate))
                {
                    taskList.Add(_ontraPlans.DeleteAsync(plan.Id!.Value));
                }
                else // Save changes.
                {
                    changes = plan.GetChanges();
                    if (changes.Any())
                    {
                        taskList.Add(_ontraPlans.UpdateAsync(plan.Id!.Value, changes));
                    }
                }
            }

            await Task.WhenAll(taskList).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes due payments for specified client, ensuring only one request gets processed at a time to avoid concurrency and double-processing.
        /// </summary>
        /// <param name="contactId">The contact to process payments for.</param>
        /// <param name="currentDate">The current date, ensuring all calculations are done using the exact same date.</param>
        /// <returns>The result of the transaction.</returns>
        public async Task<PaymentResult?> ProcessInQueueAsync(int contactId, int callingPlanId, DateTimeOffset currentDate)
        {
            try
            {
                await _queue.WaitAsync().ConfigureAwait(false);
                return await StartProcessingAsync(contactId, callingPlanId, currentDate).ConfigureAwait(false);
            }
            finally
            {
                _queue.Release();
            }
        }

        private async Task<PaymentResult?> StartProcessingAsync(int contactId, int callingPlanId, DateTimeOffset currentDate)
        {
            var t1 = GetContactAsync(contactId);
            var t2 = GetPaymentPlansAsync(contactId);
            await Task.WhenAll(t1, t2).ConfigureAwait(false);

            var contact = t1.Result;
            var plans = t2.Result;

            if (await ResetCollectionAsync(contact, plans, callingPlanId).ConfigureAwait(false))
            {
                return await ProcessAsync(contact, plans, currentDate).ConfigureAwait(false);
            }
            return null;
        }


        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _queue.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
