using System;
using HanumanInstitute.CommonWeb.Payments;

namespace HanumanInstitute.WebStore.Models
{
    public class OrderFormProduct : ProcessOrderProduct
    {
        public string? Display { get; set; }
        public decimal? StandardPrice { get; set; }
        public bool AllowEditQuantity { get; set; } = false;
        public bool AllowRemove { get; set; } = false;

        public OrderFormProduct()
        { }

        public OrderFormProduct(string productName, decimal price = 0)
        {
            Name = productName;
            Price = price;
            StandardPrice = price;
        }

        public ProcessOrderProduct CopyBase()
        {
            return new ProcessOrderProduct()
            {
                Name = Name,
                Price = Price,
                Quantity = Quantity,
                QuantityFieldId = QuantityFieldId
            };
        }
    }
}
