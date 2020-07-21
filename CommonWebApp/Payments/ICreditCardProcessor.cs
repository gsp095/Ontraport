using System.Threading.Tasks;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Processes credit card payments.
    /// </summary>
    public interface ICreditCardProcessor
    {
        /// <summary>
        /// Processes an order via credit card.
        /// </summary>
        /// <param name="order">The order to process.</param>
        /// <returns>The payment processing status.</returns>
        Task<PaymentResult> SubmitAsync(ProcessOrder order);
    }
}
