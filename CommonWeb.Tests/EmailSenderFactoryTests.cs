using System;
using HanumanInstitute.CommonWeb.Email;
using Microsoft.Extensions.Options;
using Xunit;

namespace HanumanInstitute.CommonWeb.Tests
{
    public class EmailSenderFactoryTests
    {
        private const string AdminFrom = "admin@admin.com";
        private const string AdminFromName = "Admin";

        public static IEmailSender SetupFactory()
        {
            var options = Options.Create(new EmailConfig()
            {
                MailFrom = AdminFrom,
                MailFromName = AdminFromName,
                MailServer = "server"
            });
            return new EmailSender(options);
        }

        private static void ValidateEmailSender(IEmailMessage email)
        {
            Assert.NotNull(email);
            Assert.IsType<EmailMessage>(email);
        }

        [Fact]
        public void Create_NoParam_ValidInstance()
        {
            var factory = SetupFactory();

            var email = factory.Create();

            ValidateEmailSender(email);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("subject", "body")]
        public void Create_ParamSubjectBody_ValidInstance(string subject, string body)
        {
            var factory = SetupFactory();

            var email = factory.Create(subject, body);

            ValidateEmailSender(email);
            Assert.Equal(subject ?? "", email.Mail.Subject);
            Assert.Equal(body ?? "", email.Mail.Body);
        }
    }
}
