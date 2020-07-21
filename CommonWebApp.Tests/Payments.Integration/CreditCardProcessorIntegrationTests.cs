using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.CurrencyExchange;
using HanumanInstitute.CommonWeb.IntegrationTests;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.CommonWeb.Payments;
using HanumanInstitute.CommonWeb.Tests;
using HanumanInstitute.OntraportApi;
using LazyCache;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.CommonWeb.Payments.IntegrationTests
{
    public class CreditCardProcessorIntegrationTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public CreditCardProcessorIntegrationTests(ITestOutputHelper outputHelper)
        {
            _output = outputHelper;
        }

        private MockEmailSender MockEmail => _mockEmail ??= new MockEmailSender();
        private MockEmailSender? _mockEmail;

        public CachingService Cache => _cache ??= new CachingService();
        private CachingService? _cache;

        public CurrencyConverterHttpClient CurrencyClient => _currencyClient ??= new CurrencyConverterHttpClient();
        private CurrencyConverterHttpClient? _currencyClient;

        public ICurrencyConverter CurrencyConverter => _currencyConverter ??= new CurrencyConverter(Options.Create(new CurrencyConverterConfig()), Cache, CurrencyClient, null);
        private CurrencyConverter? _currencyConverter;

        public OntraportHttpClient OntraClient => _ontraClient ??= IntegrationConfig.GetOntraportHttpClient();
        private OntraportHttpClient? _ontraClient;

        public BluePayHttpClient BluePayClient => _bluePayClient ??= new BluePayHttpClient();
        private BluePayHttpClient? _bluePayClient;

        public IOntraportProcessor OntraProcessor => _ontraProcessor ??= CreateOntraProcessor();
        private IOntraportProcessor? _ontraProcessor;
        private IOntraportProcessor CreateOntraProcessor()
        {
            var options = Options.Create(new PaymentProcessorConfig());
            return new OntraportProcessor(new OntraportContacts(OntraClient, new OntraportObjects(OntraClient)), new OntraportProducts(OntraClient), new OntraportTransactions(OntraClient), new CachingService(), options);
        }

        public PaymentProcessor Model => _model ??= CreatePaymentProcessor();
        private PaymentProcessor? _model;
        private PaymentProcessor CreatePaymentProcessor()
        {
            var bluePay = new BluePayProcessor(BluePayClient, new WebEnvironment(), new RandomGenerator(), IntegrationConfig.GetBluePaySandBoxConfig());
            var creditCard = new CreditCardProcessor(bluePay);
            var invoiceSender = new InvoiceSender(MockEmail);
            return new PaymentProcessor(MockEmail, CurrencyConverter, OntraProcessor, invoiceSender, creditCard, null, new RandomGenerator());
        }

        private const string CardNumber = "4111111111111111";
        private const string ClientEmail = "a@test.com";

        private static ProcessOrder CreateOrder(Currency currency = Currency.Usd)
        {
            return new ProcessOrder()
            {
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentCurrency = currency,
                CreditCard = new ProcessOrderCreditCard()
                {
                    CardNumber = CardNumber,
                    ExpirationMonth = 12,
                    ExpirationYear = 25,
                    SecurityCode = "123"
                },
                Address = new ProcessOrderAddress()
                {
                    FirstName = "Etienne",
                    LastName = "Charland",
                    Email = ClientEmail,
                    Address = "1082 Main Street North",
                    City = "Richmond",
                    State = "QC",
                    Country = "Canada",
                    PostalCode = "J0B2H0"
                }
            };
        }

        private static ProcessOrderProduct AddProduct1(ProcessOrder order, int quantity = 1, decimal price = 55)
        {
            var product = new ProcessOrderProduct(OntraportProcessorTests.Product1, price)
            {
                Quantity = quantity
            };
            order.Products.Add(product);
            return product;
        }

        private static ProcessOrderProduct AddProduct2(ProcessOrder order, int quantity = 1, decimal price = 100)
        {
            var product = new ProcessOrderProduct(OntraportProcessorTests.Product2, price)
            {
                Quantity = quantity
            };
            order.Products.Add(product);
            return product;
        }

        private static void SetupCreditCardFailed(ProcessOrder order)
        {
            order.CreditCard!.ExpirationYear = 01;
        }

        private void AssertResponse(PaymentResult response, PaymentStatus expectedStatus)
        {
            _output.WriteLine(response.Message);
            Assert.Equal(expectedStatus, response.Status);
        }

        [Fact]
        public async Task SubmitAsync_CurrencyEur_ThrowsException()
        {
            var order = CreateOrder(Currency.Eur);
            AddProduct1(order);

            Task Act()
            {
                return Model.SubmitAsync(order);
            }

            await Assert.ThrowsAsync<ArgumentException>(Act);
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
            // Make sure we have odd total after conversion
            var total = order.Products.CalculateTotal();
            var converted = await Model.ConvertTotalAsync(total, currency);
            if ((int)converted % 2 == 0)
            {
                order.Products[0].Price += .8m;
            }

            var result = await Model.SubmitAsync(order);

            AssertResponse(result, PaymentStatus.Approved);
        }

        [Fact]
        public async Task SubmitAsync_WithQuantity_ReturnsSuccess()
        {
            var order = CreateOrder();
            AddProduct1(order, 3);

            var result = await Model.SubmitAsync(order);

            AssertResponse(result, PaymentStatus.Approved);
        }

        [Fact]
        public async Task SubmitAsync_MultipleProductsWithQuantity_ReturnsSuccess()
        {
            var order = CreateOrder();
            AddProduct2(order, 2);
            AddProduct1(order, 3, 99); // Total needs to be odd for BluePay to return success.

            var result = await Model.SubmitAsync(order);

            AssertResponse(result, PaymentStatus.Approved);
        }

        //[Fact]
        //public async Task SubmitAsync_WithQuantity_LogsTransaction()
        //{
        //    var order = CreateOrder();
        //    AddProduct1(order, 2);

        //    var result = await Model.SubmitAsync(order);

        //    MockOntraProcessor.Verify(x => x.LogTransactionAsync(It.IsAny<ProcessOrder>()));
        //    AssertResponse(result, PaymentStatus.Success);
        //}

        [Fact]
        public async Task SubmitAsync_PaymentDeclined_StatusDeclined()
        {
            var order = CreateOrder();
            AddProduct1(order);
            SetupCreditCardFailed(order);

            var result = await Model.SubmitAsync(order);

            AssertResponse(result, PaymentStatus.Declined);
        }

        [Fact]
        public async Task SubmitAsync_PaymentDeclined_HasMessage()
        {
            var order = CreateOrder();
            AddProduct1(order);
            SetupCreditCardFailed(order);

            var result = await Model.SubmitAsync(order);

            Assert.NotEmpty(result.Message);
        }



        private bool _disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _bluePayClient?.Dispose();
                    //_ontraClient?.Dispose();
                    _currencyClient?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
