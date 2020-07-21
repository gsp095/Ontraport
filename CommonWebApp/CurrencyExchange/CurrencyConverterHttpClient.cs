using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HanumanInstitute.CommonWeb.CurrencyExchange
{
    /// <summary>
    /// Represents a connection to get exchange rates.
    /// </summary>
    public class CurrencyConverterHttpClient : HttpClient
    {
        public CurrencyConverterHttpClient()
        {

        }

        public static Uri ServiceUrl => new Uri("https://openexchangerates.org/api/latest.json?app_id=cce8f218b1e241f9b52752ee1aac76d2");

        /// <summary>
        /// Queries the server to get exchange rates.
        /// </summary>
        /// <returns></returns>
        public Task<string> GetStringAsync() => GetStringAsync(ServiceUrl);
    }
}
