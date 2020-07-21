using System;
using System.Collections.Generic;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a subscription purchase from a user.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// The ID of the order.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The ID of the user who made the purchase.
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// The associated customer.
        /// </summary>
        public Customer Customer { get; set; }
        /// <summary>
        /// The invoice number, for reference only.
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// The total of the order.
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// A promotional coupon code for discounts.
        /// </summary>
        public string CouponCode { get; set; }
        /// <summary>
        /// The UTC date of the purchase.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// The UTC date when the subscription expires.
        /// </summary>
        public DateTime? SubscriptionExpires { get; set; }
        /// <summary>
        /// The length of the subscription in months.
        /// </summary>
        public int SubscriptionLength { get; set; }
        /// <summary>
        /// The list of associated order details.
        /// </summary>
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
