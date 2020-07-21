using System;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Contains credit card information to process a transaction.
    /// </summary>
    public class ProcessOrderCreditCard
    {
        [DataType(DataType.CreditCard)]
        [Display(Name = "Card number"), Required]
        public string CardNumber { get; set; } = string.Empty;
        [Display(Name = "Expiration date"), Required]
        public int? ExpirationMonth { get; set; }
        [Display(Name = "Expiration year"), Required]
        public int? ExpirationYear { get; set; }
        [RegularExpression("^[0-9]{3,4}$", ErrorMessage = "Security code is invalid")]
        [Display(Name = "Security code"), Required]
        public string SecurityCode { get; set; } = string.Empty;
    }
}
