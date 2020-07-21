using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.CommonWeb.Payments;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Manages the processing of recurring payments.
    /// </summary>
    public interface IPaymentPlanProcessor
    {
        /// <summary>
        /// Returns the Ontraport data for specified contact.
        /// </summary>
        /// <param name="contactId">The contact to retrieve information for.</param>
        /// <returns>The contact information.</returns>
        Task<ApiCustomContact> GetContactAsync(int contactId);
        /// <summary>
        /// Returns all recurring payment plans for specified contact.
        /// </summary>
        /// <param name="contactId">The contact to retrieve payment plans for.</param>
        /// <returns>The list of payment plans.</returns>
        Task<IEnumerable<ApiPaymentPlan>> GetPaymentPlansAsync(int contactId);
        /// <summary>
        /// Returns the list of products to settle due payments.
        /// </summary>
        /// <param name="contact">The contact to create the bill for.</param>
        /// <param name="planList">The list of due payment plans.</param>
        /// <param name="currentDate">The current date, ensuring all calculations are done using the exact same date.</param>
        /// <returns>The list of products to pay.</returns>
        IList<ProcessOrderProduct> GetBillProducts(ApiCustomContact contact, IEnumerable<ApiPaymentPlan> planList, DateTimeOffset currentDate);
        /// <summary>
        /// When in collection, disables the Ontraport Collection campaign for all due payment plans except the Collection Plan ID, to avoid double-processing. Returns whether this request should be processed.
        /// </summary>
        /// <param name="contact">The contact to process payment for.</param>
        /// <param name="planList">The list of payment plans that the contact has.</param>
        /// <param name="callingPlanId">The id of the Ontraport payment plan that is calling this function via WebHook.</param>
        /// <returns>Whether this request should be processed.</returns>
        Task<bool> ResetCollectionAsync(ApiCustomContact contact, IEnumerable<ApiPaymentPlan> planList, int callingPlanId);
        /// <summary>
        /// Processes the payment for specified payment plans.
        /// </summary>
        /// <param name="contact">The contact to process payments for.</param>
        /// <param name="planList">The payment plans to process.</param>
        /// <param name="currentDate">The current date, ensuring all calculations are done using the exact same date.</param>
        /// <returns>The result of the transaction.</returns>
        Task<PaymentResult> ProcessAsync(ApiCustomContact contact, IEnumerable<ApiPaymentPlan> planList, DateTimeOffset currentDate);
        /// <summary>
        /// Processes due payments for specified client, ensuring only one request gets processed at a time to avoid concurrency and double-processing.
        /// </summary>
        /// <param name="contactId">The contact to process payments for.</param>
        /// <param name="callingPlanId">The ID of the payment plan calling this request.</param>
        /// <param name="currentDate">The current date, ensuring all calculations are done using the exact same date.</param>
        /// <returns>The result of the transaction.</returns>
        Task<PaymentResult?> ProcessInQueueAsync(int contactId, int callingPlanId, DateTimeOffset currentDate);
    }
}
