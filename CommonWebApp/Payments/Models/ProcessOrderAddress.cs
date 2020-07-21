using System;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Contains address information to process a transaction.
    /// </summary>
    public class ProcessOrderAddress
    {
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        [Required, Display(Name = "Email"), DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Display(Name = "Address Line 2")]
        public string? Address2 { get; set; }
        [Required]
        public string City { get; set; } = string.Empty;
        [Required, Display(Name = "Zip / Postal Code")]
        public string PostalCode { get; set; } = string.Empty;
        [Display(Name = "State / Province")]
        public string State { get; set; } = string.Empty;
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Referral { get; set; } = string.Empty;
    }
}
