using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Email;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.CommonWeb.Tests
{
    public class MockEmailMessage : EmailMessage
    {
        public MockEmailMessage(IOptions<EmailConfig> emailConfig) : base(emailConfig)
        {
        }

        public override void Send()
        {
            base.FillDefaultAddress();
        }

        public override Task SendAsync()
        {
            base.FillDefaultAddress();
            return Task.CompletedTask;
        }
    }
}
