using System;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.CommonWeb.Email
{
    /// <summary>
    /// Creates new instances of EmailSender
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IOptions<EmailConfig> _emailConfig;

        public EmailSender(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig;
        }

        /// <summary>
        /// Creates a new instance of EmailSender to send an email.
        /// </summary>
        /// <returns>A new IEmailSender instance.</returns>
        public IEmailMessage Create() => new EmailMessage(_emailConfig);

        /// <summary>
        /// Creates a new instance of EmailSender to send an email with specified subject and body.
        /// </summary>
        /// <param name="subject">The email's subject.</param>
        /// <param name="body">The email's body.</param>
        /// <returns>A new IEmailSender instance.</returns>
        public IEmailMessage Create(string subject, string body) => new EmailMessage(_emailConfig).SetMessage(subject, body);
    }
}
