using System.Threading.Tasks;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Sends invoices of client purchases.
    /// </summary>
    public interface IInvoiceSender
    {
        /// <summary>
        /// Sends the invoice for the purchase to the client and to the admin.
        /// </summary>
        /// <param name="order">The order for which to send an invoice.</param>
        /// <param name="invoiceId">The invoice number to display on the invoice.</param>
        Task SendInvoiceAsync(ProcessOrder order, int invoiceId);
    }
}
