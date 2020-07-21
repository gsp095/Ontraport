using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a customer shopping cart while he is applying for a subscription.
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// Gets or sets the ID of the cart.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the ID of the customer doing the purchase.
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the associated customer.
        /// </summary>
        public Customer Customer { get; set; }
        /// <summary>
        /// Gets or sets the date this cart was last updated.
        /// </summary>
        public DateTime ModifiedDate { get; set; }
        /// <summary>
        /// Gets or sets the subscription length in months.
        /// </summary>
        public int SubscriptionLength { get; set; }
        /// <summary>
        /// Gets or sets a promotional coupon code for discounts.
        /// </summary>
        public string CouponCode { get; set; }
        /// <summary>
        /// Gets or sets the list of associated cart trials.
        /// </summary>
        public List<CartTrial> CartTrials { get; set; }
        /// <summary>
        /// Gets or sets the list of associated cart products.
        /// </summary>
        public List<CartProduct> CartProducts { get; set; }
        /// <summary>
        /// Gets or sets the list of associated cart pictures.
        /// </summary>
        public List<CartPicture> CartPictures { get; set; }
    }
}
