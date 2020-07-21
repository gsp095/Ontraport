using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a product added to the cart.
    /// </summary>
    public class CartProduct
    {
        /// <summary>
        /// Gets or sets the CartProrduct primary key.
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
        /// Gets or sets the product added to the cart.
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Gets or sets the associated product.
        /// </summary>
        public Product Product { get; set; }
    }
}
