using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Payments;
using LazyCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Res = HanumanInstitute.CommonWebApp.Properties.Resources;

namespace HanumanInstitute.CommonWeb.CurrencyExchange
{
    /// <summary>
    /// Queries and caches exchange rates from an online exchange service.
    /// </summary>
    public class CurrencyConverter : ICurrencyConverter
    {
        public static Uri ServiceUrl => new Uri("https://openexchangerates.org/api/latest.json?app_id=cce8f218b1e241f9b52752ee1aac76d2");

        private readonly IOptions<CurrencyConverterConfig> _config;
        private readonly IAppCache _cache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyConverter>? _logger;

        public CurrencyConverter(IOptions<CurrencyConverterConfig> config, IAppCache cache, HttpClient httpClient, ILogger<CurrencyConverter>? logger)
        {
            _config = config;
            _cache = cache;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Returns the conversation rate and stores it into a cache for 6 hours.
        /// </summary>
        public async Task<decimal> GetRateAsync(Currency convertFrom, Currency convertTo)
        {
            return await _cache.GetOrAddAsync<decimal>(
                GetCacheKey(convertFrom, convertTo),
                () => QueryExchangeRateAsync(convertFrom, convertTo),
                TimeSpan.FromHours(_config.Value.CacheHours)).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a key string that will be used for the cache.
        /// </summary>
        /// <param name="convertFrom">The currency to convert from.</param>
        /// <param name="convertTo">The currency to convert to.</param>
        /// <returns>The cache key.</returns>
        private static string GetCacheKey(Currency convertFrom, Currency convertTo) => $"CurrencyConverter.{convertFrom}{convertTo}";

        /// <summary>
        /// Converts specified amount based on today's exchange rate.
        /// </summary>
        /// <param name="amount">The amount to convert.</param>
        /// <param name="convertFrom">The currency to convert from.</param>
        /// <param name="convertTo">The currency to convert to.</param>
        /// <returns>The converted amount.</returns>
        public async Task<decimal> ConvertAsync(decimal amount, Currency convertFrom, Currency convertTo)
        {
            return Math.Round(amount * await GetRateAsync(convertFrom, convertTo).ConfigureAwait(false), 2);
        }

        /// <summary>
        /// Queries the online service for an exchange rate
        /// </summary>
        /// <param name="convertFrom">The currency to convert from.</param>
        /// <param name="convertTo">The currency to convert to.</param>
        /// <returns>The exchange rate between the two currencies.</returns>
        private async Task<decimal> QueryExchangeRateAsync(Currency convertFrom, Currency convertTo)
        {
            if (convertFrom != Currency.Usd)
            {
                throw new NotSupportedException(Res.CurrencyConverterSupportsOnlyUsd);
            }

            // ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            _logger?.LogInformation($"Querying exchange rate from {convertFrom} to {convertTo}.");

            try
            {
                // Retry is configured with Polly when configuring services.
                var response = await _httpClient.GetStringAsync(ServiceUrl).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<CurrencyResponse>(response) ?? new CurrencyResponse(); // Suppress uninitialized class warning.
                return result.rates[convertTo.ToString().ToUpperInvariant()];
            }
            catch (WebException)
            {
                throw new WebException(Res.CurrencyConverterFailed);
            }
        }

        /// <summary>
        /// Matches the JSon model returned from the web service.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reviewed: this class is used only for parsing JSon")]
        private class CurrencyResponse
        {
            public string? disclaimer { get; set; }
            public string? license { get; set; }
            public string? timestamp { get; set; }
            public string? @base { get; set; }
            public Dictionary<string, decimal> rates { get; private set; } = new Dictionary<string, decimal>();
        }
    }
}
