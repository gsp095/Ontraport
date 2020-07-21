using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Contains transaction details to process an order.
    /// </summary>
    public class ProcessOrder
    {
        public ProcessOrder()
        { }

        public ProcessOrder SetProducts(IEnumerable<ProcessOrderProduct> products)
        {
            Products.Clear();
            Products.AddRange(products);
            return this;
        }

        /// <summary>
        /// Gets or sets the coupon code for discounts.
        /// </summary>
        public string? CouponCode { get; set; }
        /// <summary>
        /// Gets or sets the payment method.
        /// </summary>
        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }
        /// <summary>
        /// Gets the payment currency.
        /// </summary>
        public Currency PaymentCurrency { get; set; }

        /// <summary>
        /// Gets or sets the credit card information.
        /// </summary>
        public ProcessOrderCreditCard? CreditCard { get; set; }
        /// <summary>
        /// Gets or sets the credit card MasterID for recurring transactions.
        /// </summary>
        public int? CreditCardMasterId { get; set; }
        /// <summary>
        /// Gets or sets the address information.
        /// </summary>
        [Required]
        public ProcessOrderAddress Address { get; set; } = new ProcessOrderAddress();

        /// <summary>
        /// Gets or sets the list of products to purchase.
        /// </summary>
        public List<ProcessOrderProduct> Products { get; private set; } = new List<ProcessOrderProduct>();

        /// <summary>
        /// Returns the total of the order.
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// Gets or sets the order total after currency conversion.
        /// </summary>
        public decimal TotalConverted { get; set; }
        /// <summary>
        /// Gets or sets the discounts amount.
        /// </summary>
        // public decimal Discount { get; set; }
        /// <summary>
        /// Gets or sets the Ontraport FormId if processing the transaction through posting an Ontraport form.
        /// </summary>
        public string OntraportFormId { get; set; } = string.Empty;
    }
}
