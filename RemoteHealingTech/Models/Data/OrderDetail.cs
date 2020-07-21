using System;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents product purchase details within an order.
    /// </summary>
    public class OrderDetail
    {
        /// <summary>
        /// The ID of the order details.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The ID of the order.
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// The associated order.
        /// </summary>
        public Order Order { get; set; }
        /// <summary>
        /// The ID of the product purchased.
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// The associated product.
        /// </summary>
        public Product Product { get; set; }
        /// <summary>
        /// The price paid for the product.
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// The Url of the picture associated with the subscription.
        /// </summary>
        public string PictureUrl { get; set; }
    }
}
