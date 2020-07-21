using System;
using System.Collections.Generic;
using System.Linq;

namespace HanumanInstitute.CommonWeb.Payments
{
    public static class ProcessOrderProductListExtensions
    {
        /// <summary>
        /// Calculates the total of the order.
        /// </summary>
        /// <param name="products">The list of purchased products.</param>
        /// <returns>The order total.</returns>
        public static decimal CalculateTotal(this IEnumerable<ProcessOrderProduct> products)
        {
            return Math.Max(0, products.Sum(p => p.Quantity * p.Price));
        }

        /// <summary>
        /// Applies a discount to an order by reducing product prices.
        /// </summary>
        /// <param name="products">The order on which to apply a discount.</param>
        /// <param name="discount">The amount to deduct from the products.</param>
        /// <returns>Returns the amount of unused discount.</returns>
        public static decimal ApplyDiscount(this IList<ProcessOrderProduct> products, decimal discount)
        {
            products.CheckNotNull(nameof(products));
            discount.CheckRange(nameof(discount), min: 0);

            var edited = false;
            while (discount > 0 && GetProductWithPrice(out var product))
            {
                if (product.Quantity > 1)
                {
                    // Split product so we edit the price of only 1 item.
                    product.Quantity--;
                    product = new ProcessOrderProduct(product.Name, product.Price);
                    products.Add(product);
                }

                var reduce = Math.Min(discount, product.Price);
                product.Price -= reduce;
                discount -= reduce;
                edited = true;
            }
            if (edited)
            {
                GroupDuplicates(products);
            }
            return discount;

            bool GetProductWithPrice(out ProcessOrderProduct product)
            {
                product = products.FirstOrDefault(x => x.Price > 0);
                return product != null;
            }
        }

        /// <summary>
        /// Groups identical products together by increasing their quantity.
        /// </summary>
        /// <param name="products">The list of products to group.</param>
        public static IList<ProcessOrderProduct> GroupDuplicates(this IList<ProcessOrderProduct> products)
        {
            products.CheckNotNull(nameof(products));
            if (!products.Any()) { return products; }

            var grouped = products.GroupBy(x => new { x.Name, x.Price })
                .Select(x => new ProcessOrderProduct()
                {
                    Name = x.Key.Name,
                    Price = x.Key.Price,
                    Quantity = x.Sum(x => x.Quantity)
                }).ToList();
            products.Clear();
            products.AddRange(grouped);
            return products;
        }
    }
}
