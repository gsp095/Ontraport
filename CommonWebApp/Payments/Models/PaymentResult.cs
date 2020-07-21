using System;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Contains the result of a payment operation.
    /// </summary>
    public class PaymentResult
    {
        /// <summary>
        /// Gets or sets the payment status.
        /// </summary>
        public PaymentStatus Status { get; set; }
        /// <summary>
        /// Gets or sets a message with more information about the status.
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the transaction ID.
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        public PaymentResult() { }

        public PaymentResult(PaymentStatus status, string message = "")
        {
            Status = status;
            Message = message;
        }
    }
}
