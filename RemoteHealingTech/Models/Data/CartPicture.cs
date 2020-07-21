using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a picture submitted for a product subscription.
    /// </summary>
    public class CartPicture
    {
        /// <summary>
        /// Gets or sets the CarPicture primary key.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the ID of the cart.
        /// </summary>
        public Guid CartId { get; set; }
        /// <summary>
        /// Gets or sets the associated cart.
        /// </summary>
        public Cart Cart { get; set; }
        /// <summary>
        /// Gets or sets the product being purchased.
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Gets or sets the associated product.
        /// </summary>
        public Product Product { get; set; }
        /// <summary>
        /// Gets or sets the sort index to make sure pictures remain in same order.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Gets or sets the Url of the picture associated with the subscription.
        /// </summary>
        public string PictureUrl { get; set; }
    }
}
