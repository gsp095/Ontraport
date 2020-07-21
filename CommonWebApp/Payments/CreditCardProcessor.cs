using System;
using System.Threading.Tasks;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Processes credit card payments.
    /// </summary>
    public class CreditCardProcessor : ICreditCardProcessor
    {
        private readonly IBluePayProcessor _bluepay;

        public CreditCardProcessor(IBluePayProcessor bluepay)
        {
            _bluepay = bluepay.CheckNotNull(nameof(bluepay));
        }

        /// <summary>
        /// Processes an order via credit card.
        /// </summary>
        /// <param name="order">The order to process.</param>
        /// <returns>The payment processing status.</returns>
        public async Task<PaymentResult> SubmitAsync(ProcessOrder order)
        {
            order.CheckNotNull(nameof(order));
            order.Products.CheckNotNullOrEmpty(nameof(order.Products));
            order.TotalConverted.CheckRange("order.TotalConverted", 0, false);
            if (!order.CreditCardMasterId.HasValue)
            {
                order.CreditCard.CheckNotNull(nameof(order.CreditCard));
            }

            //order.Address.CheckNotNull(nameof(order.Address));
            //order.Address.FirstName.CheckNotNullOrEmpty(nameof(order.Address.FirstName));
            //order.Address.LastName.CheckNotNullOrEmpty(nameof(order.Address.LastName));
            //order.Address.Address.CheckNotNullOrEmpty(nameof(order.Address.Address));

            _bluepay.SetCurrency(order.PaymentCurrency);

            if (order.CreditCard != null)
            {
                var card = order.CreditCard;
                _bluepay.SetCCInformation(
                    card.CardNumber,
                    card.ExpirationMonth!.Value,
                    card.ExpirationYear!.Value,
                    card.SecurityCode);
            }

            var addr = order.Address!;
            _bluepay.SetCustomerInformation(
                addr.FirstName,
                addr.LastName,
                addr.Address,
                addr.Address2,
                addr.City,
                addr.State,
                addr.PostalCode,
                addr.Country,
                addr.Phone,
                addr.Email);

            _bluepay.Sale(order.TotalConverted, order.CreditCardMasterId?.ToStringInvariant());

            var response = await _bluepay.ProcessAsync().ConfigureAwait(false);

            if (response.Status == "APPROVED")
            {
                return new PaymentResult()
                {
                    Status = PaymentStatus.Approved,
                    TransactionId = response.TransId
                };
            }
            else
            {
                return new PaymentResult(PaymentStatus.Declined, response.Message);
            }
        }
    }
}
