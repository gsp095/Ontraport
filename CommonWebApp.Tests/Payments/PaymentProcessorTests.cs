using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.CurrencyExchange;
using HanumanInstitute.CommonWeb.Tests;
using Moq;
using Xunit;

namespace HanumanInstitute.CommonWeb.Payments.Tests
{
    public class PaymentProcessorTests
    {
        private MockEmailSender MockEmail => _mockEmail ??= new MockEmailSender();
        private MockEmailSender? _mockEmail;

        public ICurrencyConverter FakeCurrencyConverter => _fakeCurrencyConverter ??= CreateCurrencyConverter();
        private ICurrencyConverter? _fakeCurrencyConverter;
        private static ICurrencyConverter CreateCurrencyConverter()
        {
            var mock = new Mock<ICurrencyConverter>();
            mock.Setup(x => x.ConvertAsync(It.IsAny<decimal>(), It.IsAny<Currency>(), It.IsAny<Currency>())).Returns<decimal, Currency, Currency>((value, cFrom, cTo) => Task.FromResult(value / 2));
            return mock.Object;
        }

        public Mock<ICreditCardProcessor> MockCreditCardProcessor => _mockCreditCardProcessor ??= CreateCreditCard();
        private Mock<ICreditCardProcessor>? _mockCreditCardProcessor;
        private static Mock<ICreditCardProcessor> CreateCreditCard()
        {
            var result = new Mock<ICreditCardProcessor>();
            result.Setup(x => x.SubmitAsync(It.IsAny<ProcessOrder>())).Returns(Task.FromResult(new PaymentResult(PaymentStatus.Approved)));
            return result;
        }
        private void SetupCreditCardFailed()
        {
            MockCreditCardProcessor.Setup(x => x.SubmitAsync(It.IsAny<ProcessOrder>())).Returns(Task.FromResult(
                new PaymentResult(PaymentStatus.Declined, "error message")
            ));
        }

        public Mock<IOntraportProcessor> MockOntraProcessor => _mockOntraProcessor ??= new Mock<IOntraportProcessor>();
        private Mock<IOntraportProcessor>? _mockOntraProcessor;

        public IPaymentProcessor Model => _model ??= CreatePaymentProcessor();
        private IPaymentProcessor? _model;
        private IPaymentProcessor CreatePaymentProcessor()
        {
            var invoice = new InvoiceSender(MockEmail);
            return new PaymentProcessor(MockEmail, FakeCurrencyConverter, MockOntraProcessor.Object, invoice, MockCreditCardProcessor.Object, null, new RandomGenerator());
        }

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

        /// Validated in CreditCardProcessor that 
        //[Fact]
        //public async Task SubmitAsync_MissingCreditCard_ThrowsException()
        //{
        //    var order = new ProcessOrder()
        //    {
        //        Address = new ProcessOrderAddress()
        //        {
        //            Email = ClientEmail
        //        },
        //        PaymentMethod = PaymentMethod.CreditCard,
        //        PaymentCurrency = Currency.Usd
        //    };
        //    AddProduct1(order);

        //    Task Act()
        //    {
        //        return Model.SubmitAsync(order);
        //    }

        //    await Assert.ThrowsAsync<ArgumentNullException>(Act);
        //}

        [Fact]
        public async Task SubmitAsync_MissingAddress_ThrowsException()
        {
            var order = new ProcessOrder()
            {
                PaymentMethod = PaymentMethod.CreditCard,
                CreditCard = new ProcessOrderCreditCard() { CardNumber = CardNumber },
                PaymentCurrency = Currency.Usd
            };
            AddProduct1(order);

            Task Act()
            {
                return Model.SubmitAsync(order);
            }

            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task SubmitAsync_NoProducts_ThrowsException()
        {
            var order = CreateOrder(Currency.Usd);

            Task Act()
            {
                return Model.SubmitAsync(order);
            }

            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task SubmitAsync_ProductPriceNegative_ThrowsException()
        {
            var order = CreateOrder(Currency.Usd);
            AddProduct1(order, 1, -1);

            Task Act()
            {
                return Model.SubmitAsync(order);
            }

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(Act);
        }


        /* 
        Success:
        1. Returns success
        2. Transaction logged (in USD)
        3. Amounts are calculated correctly
        4. Sends invoice
        Failure:
        1. Returns failure with message
        2. Send email to admin
        */

        [Theory]
        [InlineData(Currency.Usd)]
        [InlineData(Currency.Cad)]
        public async Task SubmitAsync_SetCurrency_ReturnsSuccess(Currency currency)
        {
            var order = CreateOrder(currency);
            AddProduct1(order);

            var result = await Model.SubmitAsync(order);

            Assert.Equal(PaymentStatus.Approved, result.Status);
        }

        [Fact]
        public async Task SubmitAsync_WithQuantity_ReturnsSuccess()
        {
            var order = CreateOrder();
            AddProduct1(order, 2);

            var result = await Model.SubmitAsync(order);

            Assert.Equal(PaymentStatus.Approved, result.Status);
        }

        [Fact]
        public async Task SubmitAsync_MultipleProductsWithQuantity_ReturnsSuccess()
        {
            var order = CreateOrder();
            AddProduct2(order, 2);
            AddProduct1(order, 3);

            var result = await Model.SubmitAsync(order);

            Assert.Equal(PaymentStatus.Approved, result.Status);
        }

        [Fact]
        public async Task SubmitAsync_ProductPrice0_LogSuccessButNoTransaction()
        {
            var order = CreateOrder();
            order.Products.Add(new ProcessOrderProduct("prod2"));

            var result = await Model.SubmitAsync(order);

            Assert.Equal(PaymentStatus.Approved, result.Status);
            MockCreditCardProcessor.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SubmitAsync_WithQuantity_LogsTransaction()
        {
            var order = CreateOrder();
            AddProduct1(order, 2);

            var result = await Model.SubmitAsync(order);

            MockOntraProcessor.Verify(x => x.LogTransactionAsync(It.IsAny<ProcessOrder>()));
            Assert.Equal(PaymentStatus.Approved, result.Status);
        }

        [Fact]
        public async Task SubmitAsync_MultipleProductsWithQuantity_CalculatesTotal()
        {
            var order = CreateOrder();
            AddProduct1(order, 2, 50.5m);
            AddProduct2(order, 3, 100.5m);
            var expectedTotal = 402.5m;

            _ = await Model.SubmitAsync(order);

            Assert.Equal(expectedTotal, order.Total);
            Assert.Equal(expectedTotal, order.TotalConverted);
        }

        [Fact]
        public async Task SubmitAsync_ProductWithCurrencyConversion_ConvertsTotal()
        {
            var order = CreateOrder(Currency.Cad);
            AddProduct1(order, 2, 50);
            var expectedTotal = 100m;

            _ = await Model.SubmitAsync(order);

            Assert.Equal(expectedTotal, order.Total);
            Assert.NotEqual(expectedTotal, order.TotalConverted);
        }

        [Fact]
        public async Task SubmitAsync_Valid_SendsInvoice()
        {
            var order = CreateOrder();
            AddProduct1(order);

            _ = await Model.SubmitAsync(order);

            Assert.NotEmpty(MockEmail.Instances);
        }

        [Fact]
        public async Task SubmitAsync_PaymentDeclined_StatusDeclined()
        {
            var order = CreateOrder();
            AddProduct1(order);
            SetupCreditCardFailed();

            var result = await Model.SubmitAsync(order);

            Assert.Equal(PaymentStatus.Declined, result.Status);
        }

        [Fact]
        public async Task SubmitAsync_PaymentDeclined_HasMessage()
        {
            var order = CreateOrder();
            AddProduct1(order);
            SetupCreditCardFailed();

            var result = await Model.SubmitAsync(order);

            Assert.NotEmpty(result.Message);
        }

        [Fact]
        public async Task SubmitAsync_PaymentDeclined_SendsMessageToAdmin()
        {
            var order = CreateOrder();
            AddProduct1(order);
            SetupCreditCardFailed();

            _ = await Model.SubmitAsync(order);

            Assert.Single(MockEmail.Instances);
        }
    }
}
