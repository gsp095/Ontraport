using System;
using HanumanInstitute.CommonWeb.Payments;
using Xunit;

namespace HanumanInstitute.CommonWebApp.Tests.Payments
{
    public class ProcessOrderProductListExtensionsTests
    {
        private const string CardNumber = "4111 1111 1111 1111";
        private const string ClientFirstName = "First";
        private const string ClientLastName = "Last";
        private const string ClientEmail = "a@b.c";
        private const string ClientAddress = "Address";

        private static ProcessOrder CreateOrder(Currency currency = Currency.Usd)
        {
            return new ProcessOrder()
            {
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentCurrency = currency,
                CreditCard = new ProcessOrderCreditCard()
                {
                    CardNumber = CardNumber,
                    ExpirationMonth = 01,
                    ExpirationYear = 25,
                    SecurityCode = "111"
                },
                Address = new ProcessOrderAddress()
                {
                    FirstName = ClientFirstName,
                    LastName = ClientLastName,
                    Email = ClientEmail,
                    Address = ClientAddress
                }
            };
        }

        private static ProcessOrderProduct AddProduct1(ProcessOrder order, int quantity = 1, decimal price = 50)
        {
            var product = new ProcessOrderProduct("prod1", price)
            {
                Quantity = quantity
            };
            order.Products.Add(product);
            return product;
        }

        private static ProcessOrderProduct AddProduct2(ProcessOrder order, int quantity = 1, decimal price = 100)
        {
            var product = new ProcessOrderProduct("prod2", price)
            {
                Quantity = quantity
            };
            order.Products.Add(product);
            return product;
        }

        [Fact]
        public void ApplyDiscount_Zero_NoChange()
        {
            var order = CreateOrder();
            AddProduct1(order, 1, 50);

            order.Products.ApplyDiscount(0);

            Assert.Equal(50, order.Products.CalculateTotal());
        }

        [Fact]
        public void ApplyDiscount_SingleProduct_ReducePrice()
        {
            var order = CreateOrder();
            AddProduct1(order, 1, 50);

            order.Products.ApplyDiscount(10);

            Assert.Equal(40, order.Products.CalculateTotal());
        }

        [Fact]
        public void ApplyDiscount_SingleProductLargeCredit_ReturnLeftover()
        {
            var order = CreateOrder();
            AddProduct1(order, 1, 50);

            var result = order.Products.ApplyDiscount(80);

            Assert.Equal(30, result);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(80.5)]
        [InlineData(100)]
        public void ApplyDiscount_Quantity2_SplitProductsReturnExpectedTotal(decimal discount)
        {
            var order = CreateOrder();
            AddProduct1(order, 2, 50);

            order.Products.ApplyDiscount(discount);

            Assert.Equal(100 - discount, order.Products.CalculateTotal());
        }

        [Theory]
        [InlineData(1, 2, 40)]
        [InlineData(1, 2, 60)]
        [InlineData(2, 1, 60)]
        [InlineData(2, 1, 110)]
        [InlineData(2, 2, 180)]
        [InlineData(2, 2, 200)]
        [InlineData(2, 2, 250)]
        [InlineData(3, 2, 300)]
        [InlineData(3, 2, 350)]
        [InlineData(5, 5, 500)]
        public void ApplyDiscount_MultipleProducts_ReducePrices(int product1Qty, int product2Qty, decimal discount)
        {
            var order = CreateOrder();
            AddProduct1(order, product1Qty, 50);
            AddProduct2(order, product2Qty, 100);

            order.Products.ApplyDiscount(discount);

            var total = (product1Qty * 50 + product2Qty * 100) - discount;
            Assert.Equal(total, order.Products.CalculateTotal());
        }

        [Theory]
        [InlineData(1, 1, 150.1)]
        [InlineData(1, 1, 160)]
        [InlineData(2, 1, 1000)]
        [InlineData(2, 2, 1000)]
        [InlineData(1, 3, 1000)]
        [InlineData(5, 5, 1000)]
        public void ApplyDiscount_MultipleProductsLargeCredit_ReturnLeftover(int product1Qty, int product2Qty, decimal discount)
        {
            var order = CreateOrder();
            AddProduct1(order, product1Qty, 50);
            AddProduct2(order, product2Qty, 100);

            var result = order.Products.ApplyDiscount(discount);

            var left = discount - (product1Qty * 50 + product2Qty * 100);
            Assert.Equal(left, result);
        }
    }
}
