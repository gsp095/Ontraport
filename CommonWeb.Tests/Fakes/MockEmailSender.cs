using System;
using System.Collections.Generic;
using HanumanInstitute.CommonWeb.Email;
using Microsoft.Extensions.Options;
using Moq;

namespace HanumanInstitute.CommonWeb.Tests
{
    public class MockEmailSender : IEmailSender
    {
        private readonly IOptions<EmailConfig> _emailConfig;

        public MockEmailSender(IOptions<EmailConfig>? emailConfig = null)
        {
            _emailConfig = emailConfig ?? Options.Create(new EmailConfig()
            {
                MailFrom = "admin@admin.com",
                MailFromName = "Admin",
                MailServer = "server"
            });
        }

        public List<IEmailMessage> Instances { get; private set; } = new List<IEmailMessage>();

        public IEmailMessage Create()
        {
            var mock = CreateMock();
            Instances.Add(mock);
            return mock;
        }

        public IEmailMessage Create(string subject, string body)
        {
            var mock = CreateMock();
            mock.SetMessage(subject, body);
            Instances.Add(mock);
            return mock;
        }

        private IEmailMessage CreateMock()
        {
            var mock = new Mock<MockEmailMessage>(_emailConfig)
            {
                CallBase = true
            };
            return mock.Object;
        }
    }
}
