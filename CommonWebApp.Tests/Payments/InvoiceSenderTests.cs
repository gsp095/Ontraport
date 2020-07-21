using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.CommonWeb.Tests;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.CommonWeb.Payments.Tests
{
    public class InvoiceSenderTests
    {
        private readonly ITestOutputHelper _output;

        public InvoiceSenderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private MockEmailSender MockEmail => _mockEmail ??= new MockEmailSender();
        private MockEmailSender? _mockEmail;

        public IInvoiceSender Model => _model ??= new InvoiceSender(MockEmail);
        private IInvoiceSender? _model;

        private static ProcessOrder CreateOrder(PaymentMethod method = PaymentMethod.CreditCard, Currency currency = Currency.Usd)
        {
            var order = new ProcessOrder()
            {
                PaymentMethod = method,
                PaymentCurrency = currency,
                Address = new ProcessOrderAddress()
                {
                    Email = ClientEmail
                }
            };
            order.Products.Add(new ProcessOrderProduct("prod1", 100));
            return order;
        }

        private const int InvoiceNumber = 123456;
        private const string ClientEmail = "a@b.c";

        [Fact]
        public async Task SendInvoiceAsync_Valid_SendsEmail()
        {
            var order = CreateOrder();

            await Model.SendInvoiceAsync(order, InvoiceNumber);

            Assert.Single(MockEmail.Instances);
            var email = MockEmail.Instances[0];
            _output.WriteLine($"Email subject: {email.Mail.Subject}");
            _output.WriteLine($"From: {email.Mail.From.Address}");
            _output.WriteLine($"To: {email.Mail.To[0].Address}");
            _output.WriteLine($"Bcc: {email.Mail.Bcc[0].Address}");
            _output.WriteLine("");
            _output.WriteLine(email.Mail.Body);
            _output.WriteLine("");
        }

        [Fact]
        public async Task SendInvoiceAsync_MultipleProducts_SendsEmailWithProduct2()
        {
            const string Product2 = "product-2";
            var order = CreateOrder();
            order.Products.Add(new ProcessOrderProduct(Product2, 5));

            await Model.SendInvoiceAsync(order, InvoiceNumber);

            Assert.Single(MockEmail.Instances);
            var email = MockEmail.Instances[0];
            EmailToOutput(email);
            Assert.Contains(Product2, email.Mail.Body, StringComparison.InvariantCulture);
        }

        [Theory]
        [InlineData(PaymentMethod.CreditCard, Currency.Cad, "Credit Card (CAD)")]
        [InlineData(PaymentMethod.PayPal, Currency.Usd, "PayPal (USD)")]
        [InlineData(PaymentMethod.PayPalForm, Currency.Usd, "PayPal (USD)")]
        public async Task SendInvoiceAsync_PaymentMethod_DisplaysCorrectPaymentMethod(PaymentMethod method, Currency currency, string expected)
        {
            var order = CreateOrder(method, currency);

            await Model.SendInvoiceAsync(order, InvoiceNumber);

            Assert.Single(MockEmail.Instances);
            var email = MockEmail.Instances[0];
            EmailToOutput(email);
            Assert.Contains(expected, email.Mail.Body, StringComparison.InvariantCulture);
        }

        private void EmailToOutput(IEmailMessage email)
        {
            _output.WriteLine($"Email subject: {email.Mail.Subject}");
            _output.WriteLine($"From: {email.Mail.From.Address}");
            _output.WriteLine($"To: {email.Mail.To[0].Address}");
            _output.WriteLine($"Bcc: {email.Mail.Bcc[0].Address}");
            _output.WriteLine("");
            _output.WriteLine(email.Mail.Body);
            _output.WriteLine("");
        }
    }
}
