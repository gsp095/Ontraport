using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using HanumanInstitute.CommonWeb.Email;
using Microsoft.Extensions.Options;
using Xunit;

namespace HanumanInstitute.CommonWeb.Tests
{
    /// <summary>
    /// We test MockEmailSender instead of EmailSender because the only difference is that Send doesn't actually send the email.
    /// </summary>
    public class EmailSenderTests
    {
        private const string AddressEmail = "a@b.c";
        private const string AddressName = "name";
        private static MailAddress TestMailAddress => new MailAddress(AddressEmail, AddressName);
        private const string AdminFrom = "admin@b.com";
        private const string AdminFromName = "Admin";

        public static IEmailSender SetupFactory()
        {
            var options = Options.Create(new EmailConfig()
            {
                MailFrom = AdminFrom,
                MailFromName = AdminFromName,
                MailServer = "server"
            });
            return new MockEmailSender(options);
        }

        private static void ValidateEmailSender(IEmailMessage email)
        {
            Assert.NotNull(email);
            Assert.IsAssignableFrom<EmailMessage>(email);
        }

        private static void ValidateAddress(MailAddress address)
        {
            Assert.Equal(AddressEmail ?? "", address.Address);
            Assert.Equal(AddressName ?? "", address.DisplayName);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("subject", "body")]
        public void SetMessage_StringParam_ValidInstance(string subject, string body)
        {
            var factory = SetupFactory();

            var email = factory.Create().SetMessage(subject, body);

            ValidateEmailSender(email);
            Assert.Equal(subject ?? "", email.Mail.Subject);
            Assert.Equal(body ?? "", email.Mail.Body);
        }

        [Fact]
        public void To_StringParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().To(AddressEmail, AddressName);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.To);
            ValidateAddress(email.Mail.To.First());
        }

        [Fact]
        public void To_AddressParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().To(TestMailAddress);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.To);
            ValidateAddress(email.Mail.To.First());
        }

        [Fact]
        public void From_StringParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().From(AddressEmail, AddressName);

            ValidateEmailSender(email);
            Assert.NotNull(email.Mail.From);
            ValidateAddress(email.Mail.From);
        }

        [Fact]
        public void From_AddressParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().From(TestMailAddress);

            ValidateEmailSender(email);
            Assert.NotNull(email.Mail.From);
            ValidateAddress(email.Mail.From);
        }

        [Fact]
        public void ReplyTo_StringParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().ReplyTo(AddressEmail, AddressName);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.ReplyToList);
            ValidateAddress(email.Mail.ReplyToList.First());
        }

        [Fact]
        public void ReplyTo_AddressParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().ReplyTo(TestMailAddress);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.ReplyToList);
            ValidateAddress(email.Mail.ReplyToList.First());
        }

        [Fact]
        public void CC_StringParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().CC(AddressEmail, AddressName);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.CC);
            ValidateAddress(email.Mail.CC.First());
        }

        [Fact]
        public void CC_AddressParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().CC(TestMailAddress);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.CC);
            ValidateAddress(email.Mail.CC.First());
        }

        [Fact]
        public void Bcc_StringParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().Bcc(AddressEmail, AddressName);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.Bcc);
            ValidateAddress(email.Mail.Bcc.First());
        }

        [Fact]
        public void Bcc_AddressParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create().Bcc(TestMailAddress);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.Bcc);
            ValidateAddress(email.Mail.Bcc.First());
        }

        [Fact]
        public void AddAlternateView_View_ViewSetInCollection()
        {
            var factory = SetupFactory();
            using var item = new AlternateView(new MemoryStream());

            var email = factory.Create().AddAlternateView(item);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.AlternateViews);
            Assert.Equal(item, email.Mail.AlternateViews.First());
        }

        [Fact]
        public void AddAttachment_View_ViewSetInCollection()
        {
            var factory = SetupFactory();
            using var item = new Attachment(new MemoryStream(), "a");

            var email = factory.Create().AddAttachment(item);

            ValidateEmailSender(email);
            Assert.Single(email.Mail.Attachments);
            Assert.Equal(item, email.Mail.Attachments.First());
        }

        [Fact]
        public void FillDefaultAddress_SetFrom_ToSetToDefault_FromUntouched()
        {
            var factory = SetupFactory();

            var email = factory.Create().From(TestMailAddress);
            email.Send();

            ValidateAddress(email.Mail.From);
            Assert.Single(email.Mail.To);
            var to = email.Mail.To.First();
            Assert.Equal(AdminFrom, to.Address);
            Assert.Equal(AdminFromName, to.DisplayName);
        }

        [Fact]
        public void FillDefaultAddress_SetTo_FromSetToDefault_ToUntouched()
        {
            var factory = SetupFactory();

            var email = factory.Create().To(TestMailAddress);
            email.Send();

            Assert.Single(email.Mail.To);
            ValidateAddress(email.Mail.To.First());
            var from = email.Mail.From;
            Assert.Equal(AdminFrom, from.Address);
            Assert.Equal(AdminFromName, from.DisplayName);
        }
    }
}
