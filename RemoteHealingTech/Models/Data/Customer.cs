using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HanumanInstitute.RemoteHealingTech.Models
{
    /// <summary>
    /// Represents a customer.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// The ID of the customer.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The associated user login.
        /// </summary>
        public ApplicationUser ApplicationUser { get; set; }
        /// <summary>
        /// Gets or sets the customer's first name.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the customer's last name.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the customer's address.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Gets or sets the customer's address line 2.
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// Gets or sets the customer's city.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Gets or sets the customer's postal code.
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// Gets or sets the customer's province.
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// Gets or sets the customer's country.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Gets or sets the customer's phone.
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Gets or sets how the uscustomerer heard about us.
        /// </summary>
        public string Reference { get; set; }
        /// <summary>
        /// Gets or sets whether subscription auto-renew is activated.
        /// </summary>
        public bool AutoRenew { get; set; }
        /// <summary>
        /// Gets or sets the customer's balance to be credited from future purchases.
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// Gets or sets the list of trials the customer subscribed to.
        /// </summary>
        public IEnumerable<Trial> Trials { get; set; }
        /// <summary>
        /// Gets or sets the list of orders purchased by the customer.
        /// </summary>
        public IEnumerable<Order> Orders { get; set; }
        /// <summary>
        /// Gets or sets shopping cart of the customer.
        /// </summary>
        public Cart Cart { get; set; }
    }
}
