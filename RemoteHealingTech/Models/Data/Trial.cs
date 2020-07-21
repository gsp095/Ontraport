using System;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a trial that a customer subscribed to.
    /// </summary>
    public class Trial
    {
        /// <summary>
        /// The ID of the trial.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The ID of the customer who subscribed to the trial.
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// The customer who subscribed to the trial.
        /// </summary>
        public Customer Customer { get; set; }
        /// <summary>
        /// The ID of the trial product.
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// The trial product.
        /// </summary>
        public Product Product { get; set; }
        /// <summary>
        /// The UTC date from which the trial takes effect.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// The UTC date when the trial expires.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
