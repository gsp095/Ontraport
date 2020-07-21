using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a product or service available as a trial or subscription.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The ID of the product.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The name of the product.
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// The description of the product.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The information page of the product.
        /// </summary>
        public string InfoPage { get; set; }
        /// <summary>
        /// The product display order will be sorted by this value.
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// The amount of days available for trial, can be 0 if it is not a subscription. Null if no trial is available.
        /// </summary>
        [Range(0, 365)]
        public int? TrialDays { get; set; }
        /// <summary>
        /// The description of the trial's length.
        /// </summary>
        public string TrialDescription { get; set; }
        /// <summary>
        /// True if the product or service is a subscription over a period of time, false if it is a one-time purchase.
        /// </summary>
        public bool IsSubscription { get; set; }
        /// <summary>
        /// Gets or sets the list of customer trials for this product.
        /// </summary>
        public IEnumerable<Trial> Trials { get; set; }
        /// <summary>
        /// Gets or sets the list of associated product trials added to cart.
        /// </summary>
        [ForeignKey("ProductId")]
        public IEnumerable<CartTrial> CartTrials { get; set; }
        /// <summary>
        /// Gets or sets the list of associated products added to cart.
        /// </summary>
        [ForeignKey("ProductId")]
        public IEnumerable<CartProduct> CartProducts { get; set; }
        /// <summary>
        /// Gets or sets the list of associated product pictures added to cart.
        /// </summary>
        [ForeignKey("ProductId")]
        public IEnumerable<CartPicture> CartPictures { get; set; }
    }
}
