using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Processes orders using the various payment methods.
    /// </summary>
    public interface IPaymentProcessor
    {
        /// <summary>
        /// Converts the total into specified currency.
        /// </summary>
        /// <param name="total">The amount to convert in USD.</param>
        /// <param name="curTo">The currency to convert to.</param>
        /// <returns>The converted amount.</returns>
        Task<decimal> ConvertTotalAsync(decimal total, Currency curTo);
        /// <summary>
        /// Processes specified order.
        /// </summary>
        /// <param name="order">The order data.</param>
        /// <returns>The payment processing result.</returns>
        Task<PaymentResult> SubmitAsync(ProcessOrder order);
    }
}
