using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.CurrencyExchange;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.CommonWeb.Validation;
using Res = HanumanInstitute.CommonWebApp.Properties.Resources;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Processes orders using the various payment methods.
    /// </summary>
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly IEmailSender _emailSender;
        private readonly ICurrencyConverter _converter;
        private readonly IOntraportProcessor? _ontraProcessor;
        private readonly IInvoiceSender? _invoiceSender;
        private readonly ICreditCardProcessor? _creditCard;
        private readonly IPayPalFormProcessor? _paypalForm;
        private readonly IRandomGenerator? _random;

        public PaymentProcessor(IEmailSender emailSender, ICurrencyConverter converter, IOntraportProcessor? ontraProcessor, IInvoiceSender? invoiceSender, ICreditCardProcessor? creditCard, IPayPalFormProcessor? paypalForm, IRandomGenerator? random)
        {
            _emailSender = emailSender;
            _converter = converter;
            _ontraProcessor = ontraProcessor;
            _invoiceSender = invoiceSender;
            _creditCard = creditCard;
            _paypalForm = paypalForm;
            _random = random;
        }

        /// <summary>
        /// Converts the total into specified currency.
        /// </summary>
        /// <param name="total">The amount to convert in USD.</param>
        /// <param name="to">The currency to convert to.</param>
        /// <returns>The converted amount.</returns>
        public async Task<decimal> ConvertTotalAsync(decimal total, Currency curTo)
        {
            return curTo == Currency.Usd ? total :
                await _converter.ConvertAsync(total, Currency.Usd, curTo).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes an order.
        /// </summary>
        /// <param name="order">The order data.</param>
        /// <returns>The payment processing result.</returns>
        public async Task<PaymentResult> SubmitAsync(ProcessOrder order)
        {
            order.CheckNotNull(nameof(order));
            order.ValidateAndThrow();

            if (order.PaymentMethod == PaymentMethod.CreditCard && _creditCard != null)
            {
                order.Products.CheckNotNullOrEmpty(nameof(order.Products));
                order.Products.ForEach(x => x.Price.CheckRange("order.Products.Price", 0));

                order.Total = order.Products.CalculateTotal();
                order.TotalConverted = await ConvertTotalAsync(order.Total, order.PaymentCurrency).ConfigureAwait(false);

                if (order.Total > 0)
                {
                    var result = await _creditCard.SubmitAsync(order).ConfigureAwait(false);
                    if (result.Status == PaymentStatus.Approved)
                    {
                        await LogTransactionAsync(order, result, true).ConfigureAwait(false);
                    }
                    else
                    {
                        // Send admin notification on error.
                        var addr = order.Address!;
                        var body = $"First Name: {addr.FirstName}<br>Last Name: {addr.LastName}<br>Email: {addr.Email}<br><br>Error: {result.Message}";
                        await _emailSender.Create("Shopping Cart Error", body).SendAsync().ConfigureAwait(false);
                    }
                    return result;
                }
                else
                {
                    await LogTransactionAsync(order, new PaymentResult(PaymentStatus.Approved), false).ConfigureAwait(false);
                    return new PaymentResult(PaymentStatus.Approved);
                }
            }
            else if (order.PaymentMethod == PaymentMethod.PayPalForm && _paypalForm != null)
            {
                var result = _paypalForm!.Submit(order);
                return new PaymentResult(PaymentStatus.Approved, result);
            }
            else
            {
                throw new InvalidOperationException(Res.PaymentMethodNotHandled);
            }
        }

        /// <summary>
        /// Logs a transaction in Ontraport, and optionally sends an invoice.
        /// </summary>
        /// <param name="order">The order to send.</param>
        /// <param name="result">The transaction result.</param>
        /// <param name="sendInvoice">Whether to send an invoice.</param>
        private async Task LogTransactionAsync(ProcessOrder order, PaymentResult result, bool sendInvoice)
        {
            await _ontraProcessor!.LogTransactionAsync(order).ConfigureAwait(false);

            if (sendInvoice && _invoiceSender != null)
            {
                // Use last 6 digits of transaction ID as invoice number.
                var invoiceId = (result.TransactionId.Length >= 6) ?
                    int.Parse(result.TransactionId.Substring(result.TransactionId.Length - 6), CultureInfo.InvariantCulture) :
                    _random?.GetDigits(6) ?? 0;
                await _invoiceSender.SendInvoiceAsync(order, invoiceId).ConfigureAwait(false);
            }
        }
    }
}
