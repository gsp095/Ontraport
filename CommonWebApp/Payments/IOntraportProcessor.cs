using System.Collections.Generic;
using System.Threading.Tasks;
using HanumanInstitute.OntraportApi.Models;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Logs transactions into Ontraport.
    /// </summary>
    public interface IOntraportProcessor
    {
        /// <summary>
        /// Creates the transaction offer by fetching product IDs from Ontraport.
        /// </summary>
        /// <param name="products">The list of products to add to the offer.</param>
        /// <returns>The ApiTransactionOffer.</returns>
        Task<ApiTransactionOffer> CreateTransactionOfferAsync(IList<ProcessOrderProduct> products);

        /// <summary>
        /// Logs a transaction into Ontraport.
        /// </summary>
        /// <param name="order">The transaction data.</param>
        Task LogTransactionAsync(ProcessOrder order);
    }
}