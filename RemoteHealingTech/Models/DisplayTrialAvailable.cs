using System;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a product trial, containing extra information for display purpose.
    /// </summary>
    public class DisplayTrialAvailable : Product
    {
        /// <summary>
        /// Gets or whether the product trial is available for the current user.
        /// </summary>
        public bool TrialAvailable { get; set; }
        /// <summary>
        /// The message to display next to the product.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets whether this product has been selected to start a trial.
        /// </summary>
        public bool Selected { get; set; }

        public DisplayTrialAvailable()
        {
        }

        public DisplayTrialAvailable(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            InfoPage = product.InfoPage;
            DisplayOrder = product.DisplayOrder;
            TrialDays = product.TrialDays;
            IsSubscription = product.IsSubscription;
        }
    }
}
