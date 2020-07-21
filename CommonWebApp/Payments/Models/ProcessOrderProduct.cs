using System;
using System.ComponentModel.DataAnnotations;
using HanumanInstitute.CommonWebApp;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Contains information about a product to purchase.
    /// </summary>
    public class ProcessOrderProduct
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Range(0, 100000)]
        public decimal Price { get; set; }
        [Range(0, 100)]
        public int Quantity { get; set; } = 1;
        public string? QuantityFieldId { get; set; }

        public ProcessOrderProduct() { }

        public ProcessOrderProduct(string name, decimal price = 0)
        {
            Name = name;
            Price = price;
        }
    }
}
