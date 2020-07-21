using System;
using HanumanInstitute.CommonWeb.CurrencyExchange;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.OntraportApi;
using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Net.Http;

namespace HanumanInstitute.CommonWeb.Payments.Tests
{
    public class PayPalFormProcessorTests
    {
        private Mock<IEmailSender> MockEmail => _mockEmail ??= new Mock<IEmailSender>();
        private Mock<IEmailSender>? _mockEmail;

        public Mock<ICurrencyConverter> FakeCurrencyConverter => _fakeCurrencyConverter ??= new Mock<ICurrencyConverter>();
        private Mock<ICurrencyConverter>? _fakeCurrencyConverter;

        public PaymentProcessor Model => _model ??= CreatePaymentProcessor();
        private PaymentProcessor? _model;
        private PaymentProcessor CreatePaymentProcessor()
        {
            var ontraForms = new OntraportPostForms(new HttpClient());
            var paypal = new PayPalFormProcessor(ontraForms);
            return new PaymentProcessor(MockEmail.Object, FakeCurrencyConverter.Object, null, null, null, paypal, null);
        }

        private const string OntraFormId = "A5555TEST";

        [Fact]
        public async Task SubmitAsync_CurrencyCad_ThrowsException()
        {
            var order = new ProcessOrder()
            {
                PaymentMethod = PaymentMethod.PayPalForm,
                PaymentCurrency = Currency.Cad,
                OntraportFormId = OntraFormId,
                Address = new ProcessOrderAddress()
            };
            order.Products.Add(new ProcessOrderProduct("prod1", 100));

            var act = Model.SubmitAsync(order);

            await Assert.ThrowsAsync<ArgumentException>(async () => await act);
        }

        [Fact]
        public async Task SubmitAsync_MissingFormId_ThrowsException()
        {
            var order = new ProcessOrder()
            {
                PaymentMethod = PaymentMethod.PayPalForm,
                PaymentCurrency = Currency.Usd,
                Address = new ProcessOrderAddress()
            };
            order.Products.Add(new ProcessOrderProduct("prod1", 100));

            var act = Model.SubmitAsync(order);

            await Assert.ThrowsAsync<ArgumentException>(async () => await act);
        }


        [Fact]
        public async Task SubmitAsync_CurrencyUsd_ResultContainsFormId()
        {
            var order = new ProcessOrder()
            {
                PaymentMethod = PaymentMethod.PayPalForm,
                PaymentCurrency = Currency.Usd,
                OntraportFormId = OntraFormId,
                Address = new ProcessOrderAddress()
            };
            order.Products.Add(new ProcessOrderProduct("prod1", 100));

            var result = await Model.SubmitAsync(order);

            Assert.Equal(PaymentStatus.Approved, result.Status);
            Assert.NotEmpty(result.Message);
            Assert.Contains(OntraFormId, result.Message, StringComparison.InvariantCulture);
        }

        [Fact]
        public async Task SubmitAsync_NoProducts_ResultSuccess()
        {
            var order = new ProcessOrder()
            {
                PaymentMethod = PaymentMethod.PayPalForm,
                PaymentCurrency = Currency.Usd,
                OntraportFormId = OntraFormId,
                Address = new ProcessOrderAddress()
            };

            var result = await Model.SubmitAsync(order);

            Assert.Equal(PaymentStatus.Approved, result.Status);
        }

        [Fact]
        public async Task SubmitAsync_SetQuantity_ThrowsException()
        {
            var order = new ProcessOrder()
            {
                PaymentMethod = PaymentMethod.PayPalForm,
                PaymentCurrency = Currency.Usd,
                OntraportFormId = OntraFormId,
                Address = new ProcessOrderAddress()
            };
            order.Products.Add(new ProcessOrderProduct("prod1", 100)
            {
                Quantity = 2
            });

            var act = Model.SubmitAsync(order);

            await Assert.ThrowsAsync<NullReferenceException>(async () => await act);
        }

        [Fact]
        public async Task SubmitAsync_SetQuantityId_ResultContainsQuantityId()
        {
            var qtyIdField = "QtyField1";
            var order = new ProcessOrder()
            {
                PaymentMethod = PaymentMethod.PayPalForm,
                PaymentCurrency = Currency.Usd,
                OntraportFormId = OntraFormId,
                Address = new ProcessOrderAddress()
            };
            order.Products.Add(new ProcessOrderProduct("prod1", 100)
            {
                Quantity = 2,
                QuantityFieldId = qtyIdField
            });

            var result = await Model.SubmitAsync(order);

            Assert.Contains(qtyIdField, result.Message, StringComparison.InvariantCulture);
        }
    }
}
