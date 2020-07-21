using System;

namespace HanumanInstitute.CommonWeb.CurrencyExchange
{
    /// <summary>
    /// Contains configuration settings for CurrencyConverter.
    /// </summary>
    public class CurrencyConverterConfig
    {
        /// <summary>
        /// Gets or sets the cache duration in hours.
        /// </summary>
        public double CacheHours { get; set; } = 6;
    }
}
