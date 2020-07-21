using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Payments;

namespace HanumanInstitute.CommonWeb.CurrencyExchange
{
    /// <summary>
    /// Queries and caches exchange rates from an online exchange service.
    /// </summary>
    public interface ICurrencyConverter
    {
        /// <summary>
        /// Returns the conversation rate and stores it into a cache for 6 hours.
        /// </summary>
        Task<decimal> GetRateAsync(Currency convertFrom, Currency convertTo);

        /// <summary>
        /// Converts specified amount based on today's exchange rate.
        /// </summary>
        /// <param name="amount">The amount to convert.</param>
        /// <param name="convertFrom">The currency to convert from.</param>
        /// <param name="convertTo">The currency to convert to.</param>
        /// <returns>The converted amount.</returns>
        Task<decimal> ConvertAsync(decimal amount, Currency convertFrom, Currency convertTo);
    }
}
