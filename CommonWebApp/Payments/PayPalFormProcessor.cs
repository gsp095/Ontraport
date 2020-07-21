using System;
using System.Collections.Generic;
using HanumanInstitute.OntraportApi;
using Res = HanumanInstitute.CommonWebApp.Properties.Resources;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Processes payments via PayPal by submitting the sale via an Ontraport form.
    /// </summary>
    public class PayPalFormProcessor : IPayPalFormProcessor
    {
        private readonly IOntraportPostForms _ontraForms;

        public PayPalFormProcessor(IOntraportPostForms ontraForms)
        {
            _ontraForms = ontraForms;
        }

        /// <summary>
        /// Submits the sale via Ontraport form.
        /// </summary>
        public string Submit(ProcessOrder order)
        {
            order.CheckNotNull(nameof(order));
            order.OntraportFormId.CheckNotNullOrEmpty(nameof(order.OntraportFormId));
            order.Address.CheckNotNull(nameof(order.Address));
            if (order.PaymentCurrency != Currency.Usd) { throw new ArgumentException(Res.PayPalFormInvalidCurrency); }

            return _ontraForms.ClientPost(order.OntraportFormId, GetFormData(order));
        }

        /// <summary>
        /// Returns the form data as a collection.
        /// </summary>
        /// <param name="includePayment">Whether or not to include payment fields.</param>
        /// <returns>A NameValueConnection containing the form data.</returns>
        private static IDictionary<string, object?> GetFormData(ProcessOrder order)
        {
            var addr = order.Address;
            var result = new Dictionary<string, object?>() {
                { "firstname", addr!.FirstName },
                { "lastname", addr.LastName },
                { "email", addr.Email },
                { "billing_address1", addr.Address },
                { "billing_address2", addr.Address2 },
                { "billing_city", addr.City },
                { "billing_zip", addr.PostalCode },
                { "billing_state", string.IsNullOrEmpty(addr.State) ? "_NOTLISTED_" : addr.State },
                { "billing_country", addr.Country },
                { "office_phone", addr.Phone },
                { "f1360", addr.Referral },
                { "paypal", order.PaymentMethod == PaymentMethod.PayPalForm ? "yes" : ""}
            };

            if (order.Products != null)
            {
                foreach (var item in order.Products)
                {
                    if (!string.IsNullOrEmpty(item.QuantityFieldId))
                    {
                        result.Add($"quantity['{item.QuantityFieldId}']", item.Quantity.ToStringInvariant());
                    }
                    else if (item.Quantity > 1 && order.PaymentMethod == PaymentMethod.PayPalForm)
                    {
                        throw new NullReferenceException(Res.PayPalFormQuantityIdNull);
                    }
                }
            }

            return result;
        }
    }
}
