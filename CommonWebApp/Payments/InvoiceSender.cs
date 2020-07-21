using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Email;
using Res = HanumanInstitute.CommonWebApp.Properties.Resources;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Sends invoices of client purchases.
    /// </summary>
    public class InvoiceSender : IInvoiceSender
    {
        private const string ResourceInvoice = "HanumanInstitute.CommonWebApp.Payments.TemplateInvoice.htm";
        private const string ResourceInvoiceItem = "HanumanInstitute.CommonWebApp.Payments.TemplateInvoiceItem.htm";

        private readonly IEmailSender _emailSender;

        public InvoiceSender(IEmailSender emailSender)
        {
            _emailSender = emailSender.CheckNotNull(nameof(emailSender));
        }

        /// <summary>
        /// Sends the invoice for the purchase to the client and to the admin.
        /// </summary>
        /// <param name="order">The order for which to send an invoice.</param>
        /// <param name="invoiceId">The invoice number to display on the invoice.</param>
        public async Task SendInvoiceAsync(ProcessOrder order, int invoiceId)
        {
            order.CheckNotNull(nameof(order));
            order.Address.CheckNotNull(nameof(order.Address));
            order.Address?.Email.CheckNotNullOrEmpty(nameof(order.Address.Email));

            var invoiceTitle = string.Format(CultureInfo.InvariantCulture, Res.InvoiceEmailTitle, invoiceId);

            // Load invoice template.
            var template = await LoadResourceTemplateAsync(ResourceInvoice).ConfigureAwait(false);
            var templateItem = await LoadResourceTemplateAsync(ResourceInvoiceItem).ConfigureAwait(false);

            var invoiceBody = new StringBuilder(template);
            var addr = order.Address;
            invoiceBody.Replace("[Invoice ID]", invoiceId.ToStringInvariant())
                .Replace("[Invoice Date]", DateTime.Now.ToString("d", CultureInfo.CurrentCulture))
                .Replace("[Payment Method]", GetPaymentMethodText(order))
                .Replace("[Email]", addr!.Email)
                .Replace("[First Name]", addr.FirstName)
                .Replace("[Last Name]", addr.LastName)
                .Replace("[Address]", addr.Address)
                .Replace("[Address 2]", addr.Address2)
                .Replace("[City]", addr.City)
                .Replace("[State]", addr.State)
                .Replace("[Zip Code]", addr.PostalCode)
                .Replace("[Country]", addr.Country)
                .Replace("[Phone]", addr.Phone);
            //if (order.Discount == 0)
            //{
            invoiceBody.Replace("[SubtotalText]", "Subtotal");
            invoiceBody.Replace("[Subtotal]", order.Total.ToString("c", CultureInfo.CurrentCulture));
            //}
            //else
            //{
            //    invoiceBody.Replace("[SubtotalText]", "Discount");
            //    invoiceBody.Replace("[Subtotal]", order.Discount.ToString("c", CultureInfo.CurrentCulture));
            //}
            invoiceBody.Replace("[Total]", order.PaymentCurrency == Currency.Usd ?
                order.Total.ToString("c", CultureInfo.CurrentCulture) :
                string.Format(CultureInfo.CurrentCulture, "{0:c} USD ({1:c} CAD)", order.Total, order.TotalConverted));

            var invoiceItems = new StringBuilder();
            foreach (var item in order.Products)
            {
                invoiceItems.AppendFormat(CultureInfo.InvariantCulture, templateItem,
                    item.Name, item.Quantity, item.Price.ToString("c", CultureInfo.CurrentCulture), (item.Quantity * item.Price).ToString("c", CultureInfo.CurrentCulture));
            }
            invoiceBody.Replace("[Items]", invoiceItems.ToString());

            // Send invoice email.
            var email = _emailSender.Create(invoiceTitle, invoiceBody.ToString());
            email.Mail.IsBodyHtml = true;
            email.To(addr.Email, $"{addr.FirstName} {addr.LastName}");
            // Send to admin.
            email.Mail.Bcc.Add(email.DefaultMailAddress);
            await email.SendAsync().ConfigureAwait(false);
        }

        private static async Task<string> LoadResourceTemplateAsync(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            throw new FileNotFoundException(Res.InvoiceTemplateNotFound.FormatInvariant(resourceName));
        }

        private static string GetPaymentMethodText(ProcessOrder order)
        {
            var result = order.PaymentMethod == PaymentMethod.CreditCard ? "Credit Card" : "PayPal";
            result += " ({0})".FormatInvariant(order.PaymentCurrency.ToStringInvariant().ToUpperInvariant());
            return result;
        }
    }
}
