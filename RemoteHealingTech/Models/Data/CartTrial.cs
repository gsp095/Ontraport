using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a trial added to the cart.
    /// </summary>
    public class CartTrial
    {
        /// <summary>
        /// Gets or sets the CartTrial primary key.
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
        /// Gets or sets the trial added to the cart.
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Gets or sets the associated product.
        /// </summary>
        public Product Product { get; set; }
    }
}
