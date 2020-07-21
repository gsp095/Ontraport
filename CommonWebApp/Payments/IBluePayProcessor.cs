using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Payments;

namespace HanumanInstitute.CommonWeb
{
    public interface IBluePayProcessor
    {
        /// <summary>
        /// Sets the currency to use for transactions. Must be called before using this class.
        /// </summary>
        /// <param name="currency">The currency to use for trarnsactions.</param>
        void SetCurrency(Currency currency);
        /// <summary>
        /// Sets Customer Information
        /// </summary>
        void SetCustomerInformation(string firstName, string lastName, string? address1 = null, string? address2 = null, string? city = null, string? state = null, string? zip = null, string? country = null, string? phone = null, string? email = null, string? companyName = null);
        /// <summary>
        /// Sets Credit Card Information
        /// </summary>
        void SetCCInformation(string ccNumber, int expMonth, int expYear, string cvv2);
        /// <summary>
        /// Runs a Sale Transaction
        /// </summary>
        void Sale(decimal amount, string? masterID = null, string? customerToken = null);
        /// <summary>
        /// Posts the transaction to the server.
        /// </summary>
        /// <returns>The transaction result.</returns>
        Task<BluePayResponse> ProcessAsync();
    }
}
