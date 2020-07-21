using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using HanumanInstitute.CommonWeb.Payments;
using HanumanInstitute.OntraportApi;
using HanumanInstitute.OntraportApi.Models;
using HanumanInstitute.CommonWeb.Ontraport;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using HanumanInstitute.CommonWeb.Tests;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using HanumanInstitute.CommonWeb.IntegrationTests;

namespace HanumanInstitute.CommonWeb.Payments.IntegrationTests
{
    public class OntraportProcessorTests
    {
        private readonly ITestOutputHelper _output;
        // These must be valid product names in Ontraport sandbox account.
        public const string Product1 = "product1", Product2 = "Prod2", Product3 = "Prod3";
        private const string TestEmail = "LogTest@test.com";

        public ILogger<OntraportHttpClient> Logger => _logger ??= TestLogger.Create<OntraportHttpClient>(_output);
        private ILogger<OntraportHttpClient>? _logger;

        public OntraportHttpClient OntraHttp => _ontraHttp ??= IntegrationConfig.GetOntraportHttpClient(_logger);
        private OntraportHttpClient? _ontraHttp;

        public OntraportContacts Contacts => _contacts ??= new OntraportContacts(OntraHttp, new OntraportObjects(OntraHttp));
        private OntraportContacts? _contacts;

        public OntraportTransactions Trans => _trans ??= new OntraportTransactions(OntraHttp);
        private OntraportTransactions? _trans;

        public OntraportProcessor Model => _model ??= CreateModel();
        private OntraportProcessor? _model;
        private OntraportProcessor CreateModel()
        {
            var cache = new LazyCache.Mocks.MockCachingService();
            var options = Mock.Of<IOptions<PaymentProcessorConfig>>(x => x.Value == new PaymentProcessorConfig());
            var ontraProducts = new OntraportProducts(OntraHttp);
            return new OntraportProcessor(Contacts, ontraProducts, Trans, cache, options);
        }

        public OntraportProcessorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        //private IOntraportProcessor SetupModel()
        //{
        //    //_logger = TestLogger.Create<OntraportHttpClient>(_output);
        //    //var ontraHttp = IntegrationConfig.GetOntraportHttpClient(_logger);
        //    //var ontraObjects = new OntraportObjects(ontraHttp);
        //    //_ontraContacts = new OntraportContacts(ontraHttp, ontraObjects);
        //    //var ontraProducts = new OntraportProducts(ontraHttp);
        //    //_ontraTrans = new OntraportTransactions(ontraHttp);

        //}

        [Fact]
        public async Task CreateTransactionOfferAsync_EmptyList_ReturnsEmptyList()
        {
            var products = new List<ProcessOrderProduct>();

            var result = await Model.CreateTransactionOfferAsync(products);

            Assert.NotNull(result);
            Assert.Empty(result.Products);
        }

        [Fact]
        public async Task CreateTransactionOfferAsync_InvalidProducts_ThrowsException()
        {
            var products = new List<ProcessOrderProduct>()
            {
                new ProcessOrderProduct(Product1),
                new ProcessOrderProduct("DOES_NOT_EXIST"),
            };

            Task Act() => Model.CreateTransactionOfferAsync(products);

            await Assert.ThrowsAsync<ArgumentException>(Act);
        }

        [Fact]
        public async Task CreateTransactionOfferAsync_ValidProducts_ReturnsProductIds()
        {
            var products = new List<ProcessOrderProduct>()
            {
                new ProcessOrderProduct(Product1)                ,
                new ProcessOrderProduct(Product2, 20),
                new ProcessOrderProduct(Product3, 100)
                {
                    Quantity = 5
                },
                new ProcessOrderProduct(Product1),
                new ProcessOrderProduct(Product2, 20),
                new ProcessOrderProduct(Product3, 100)
                {
                    Quantity = 5
                },
                new ProcessOrderProduct(Product1),
                new ProcessOrderProduct(Product2, 20),
            };

            var result = await Model.CreateTransactionOfferAsync(products);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Products);
            for (var i = 0; i < products.Count; i++)
            {
                Assert.NotEqual(0, result.Products[i].ProductId);
                Assert.Equal(products[i].Quantity, result.Products[i].Quantity);
            }
        }

        [Fact]
        public async Task LogTransactionAsync_NoProducts_ThrowsException()
        {
            var order = new ProcessOrder()
            {
                Address = new ProcessOrderAddress()
                {
                    Email = TestEmail
                }
            };

            var act = Model.LogTransactionAsync(order);

            await Assert.ThrowsAsync<ArgumentException>(() => act);
        }

        [Fact]
        public async Task LogTransactionAsync_ValidProducts_ContactHasTransaction()
        {
            var order = new ProcessOrder()
            {
                Address = new ProcessOrderAddress()
                {
                    Email = TestEmail,
                    FirstName = "Etienne",
                    LastName = "Charland"
                }
            };
            order.Products.Add(new ProcessOrderProduct(Product1, 10));
            order.Products.Add(new ProcessOrderProduct(Product2, 50));
            // Start clean, delete account with test email.
            await Contacts.DeleteAsync(new ApiSearchOptions().AddCondition("email", "=", TestEmail));

            await Model.LogTransactionAsync(order);

            var contact = await Contacts.SelectAsync(TestEmail);
            Assert.True(contact.LastInvoiceTotal > 0);
        }
    }
}
