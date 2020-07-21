using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.OntraportApi;
using HanumanInstitute.OntraportApi.Models;
using LazyCache;
using Microsoft.Extensions.Options;
using Res = HanumanInstitute.CommonWebApp.Properties.Resources;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Logs transactions into Ontraport.
    /// </summary>
    public class OntraportProcessor : IOntraportProcessor
    {
        private readonly IOntraportContacts _ontraContacts;
        private readonly IOntraportProducts _ontraProducts;
        private readonly IOntraportTransactions _ontraTransact;
        private readonly IAppCache _cache;
        private readonly IOptions<PaymentProcessorConfig> _config;

        public OntraportProcessor(IOntraportContacts ontraContacts, IOntraportProducts ontraProducts, IOntraportTransactions ontraTransact, IAppCache cache, IOptions<PaymentProcessorConfig> config)
        {
            _ontraContacts = ontraContacts;
            _ontraProducts = ontraProducts;
            _ontraTransact = ontraTransact;
            _cache = cache;
            _config = config;
        }

        /// <summary>
        /// Logs a transaction into Ontraport.
        /// </summary>
        /// <param name="order">The transaction data.</param>
        /// <param name="orderTotal">The total of the transaction.</param>
        public async Task LogTransactionAsync(ProcessOrder order)
        {
            order.CheckNotNull(nameof(order));
            order.Address.CheckNotNull(nameof(order.Address));
            order.Products.CheckNotNullOrEmpty(nameof(order.Products));

            var offerTask = CreateTransactionOfferAsync(order.Products);

            // Add/edit contact via API.
            var addr = order.Address;
            var contact = await _ontraContacts.CreateOrMergeAsync(new ApiCustomContact()
            {
                Email = addr!.Email,
                FirstName = addr.FirstName,
                LastName = addr.LastName,
                Address = addr.Address,
                Address2 = addr.Address2,
                City = addr.City,
                State = addr.State.HasValue() ? addr.State : "_NOTLISTED_",
                Zip = addr.PostalCode,
                Country = addr.Country,
                HomePhone = addr.Phone,
                HearAboutUs = addr.Referral
            }.GetChanges()).ConfigureAwait(false);

            // Add order via API.
            var contactId = contact?.Id ?? throw new WebException(Res.OntraportReturnedNullContactId);
            var offer = await offerTask.ConfigureAwait(false);
            await _ontraTransact.LogTransactionAsync(contactId, offer).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the transaction offer by fetching product IDs from Ontraport.
        /// </summary>
        /// <param name="products">The list of products to add to the offer.</param>
        /// <returns>The ApiTransactionOffer.</returns>
        public async Task<ApiTransactionOffer> CreateTransactionOfferAsync(IList<ProcessOrderProduct> products)
        {
            products.CheckNotNull(nameof(products));

            // Query server in parallel and maintain order.
            var resultList = await products.ForEachOrderedAsync(async x =>
            {
                var id = await GetProductIdAsync(x.Name).ConfigureAwait(false);
                return new ApiTransactionProduct(id, x.Quantity, x.Price);
            }).ConfigureAwait(false);

            var result = new ApiTransactionOffer();
            foreach (var item in resultList)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Returns the ID of a product, fetching from the cache or server.
        /// </summary>
        /// <param name="productName">The name of the product to get the ID for.</param>
        /// <returns>The product ID.</returns>
        private async Task<int> GetProductIdAsync(string productName)
        {
            return await _cache.GetOrAddAsync<int>(
                GetCacheKey(productName),
                async () =>
                {
                    var response = await _ontraProducts.SelectAsync(productName).ConfigureAwait(false);
                    if (response == null || !response.Id.HasValue)
                    {
                        throw new ArgumentException(Res.OntraportProductNotFound.FormatInvariant(productName));
                    }
                    return response.Id.Value;
                },
                TimeSpan.FromHours(_config.Value.ProductIdCacheHours)).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a key string that will be used for the cache.
        /// </summary>
        /// <param name="productName">The product name.</param>
        /// <returns>The cache key.</returns>
        private static string GetCacheKey(string productName) => $"PaymentProcessor.Product.{productName}";
    }
}
