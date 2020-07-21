using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.CommonWeb.Email
{
    /// <summary>
    /// Provides email sending service based on application configuration. To and From will be sent by default to the server host if not set.
    /// Properties can be set using fluent API.
    /// </summary>
    public class EmailMessage : IEmailMessage
    {
        private readonly IOptions<EmailConfig> _settings;

        /// <summary>
        /// Returns the MailMessage instance of the email to send.
        /// </summary>
        public MailMessage Mail { get; private set; } = new MailMessage();

        /// <summary>
        /// Initializes a new instance of the EmailSendingService class.
        /// </summary>
        /// <param name="settings">The configuration class.</param>
        public EmailMessage(IOptions<EmailConfig> settings)
        {
            _settings = settings.CheckNotNull(nameof(settings));
        }

        /// <summary>
        /// Sets the email subject and body.
        /// </summary>
        /// <param name="subject">The email's subject.</param>
        /// <param name="body">The email's body.</param>
        /// <returns>This class.</returns>
        public EmailMessage SetMessage(string subject, string body)
        {
            Mail.Subject = subject;
            Mail.Body = body;
            return this;
        }

        /// <summary>
        /// Adds a destination for this email message.
        /// </summary>
        /// <param name="address">The recipient's email address.</param>
        /// <param name="name">The recipient's name.</param>
        /// <returns>This class.</returns>
        public EmailMessage To(string address, string name = "")
        {
            Mail.To.Add(new MailAddress(address, name));
            return this;
        }

        /// <summary>
        /// Adds a destination for this email message.
        /// </summary>
        /// <param name="address">The recipient's email address.</param>
        /// <returns>This class.</returns>
        public EmailMessage To(MailAddress address)
        {
            Mail.To.Add(address);
            return this;
        }

        /// <summary>
        /// Sets the sender of the email message.
        /// </summary>
        /// <param name="email">The sender's email address.</param>
        /// <param name="name">The sender's name.</param>
        /// <returns>This class.</returns>
        public EmailMessage From(string email, string name = "")
        {
            Mail.From = new MailAddress(email, name);
            return this;
        }

        /// <summary>
        /// Sets the sender of the email message.
        /// </summary>
        /// <param name="address">The sender's email address.</param>
        /// <returns>This class.</returns>
        public EmailMessage From(MailAddress address)
        {
            Mail.From = address;
            return this;
        }

        /// <summary>
        /// Adds a reply to address of this message.
        /// </summary>
        /// <param name="address">The address to reply to.</param>
        /// <param name="name">The address name to reply to.</param>
        /// <returns>This class.</returns>
        public EmailMessage ReplyTo(string address, string name = "")
        {
            Mail.ReplyToList.Add(new MailAddress(address, name));
            return this;
        }

        /// <summary>
        /// Adds a reply to address to this message.
        /// </summary>
        /// <param name="address">The address to reply to.</param>
        /// <returns>This class.</returns>
        public EmailMessage ReplyTo(MailAddress address)
        {
            Mail.ReplyToList.Add(address);
            return this;
        }

        /// <summary>
        /// Adds a carbon copy (CC) recipient to the email message.
        /// </summary>
        /// <param name="address">The recipient's address.</param>
        /// <param name="name">The recipient's name.</param>
        /// <returns>This class.</returns>
        public EmailMessage CC(string address, string name = "")
        {
            Mail.CC.Add(new MailAddress(address, name));
            return this;
        }

        /// <summary>
        /// Adds a carbon copy (CC) recipient to the email message.
        /// </summary>
        /// <param name="address">The recipient's address.</param>
        /// <returns>This class.</returns>
        public EmailMessage CC(MailAddress address)
        {
            Mail.CC.Add(address);
            return this;
        }

        /// <summary>
        /// Adds a blind carbon copy (BCC) recipient to the email message.
        /// </summary>
        /// <param name="address">The recipient's address.</param>
        /// <param name="name">The recipient's name.</param>
        /// <returns>This class.</returns>
        public EmailMessage Bcc(string address, string name = "")
        {
            Mail.Bcc.Add(new MailAddress(address, name));
            return this;
        }

        /// <summary>
        /// Adds a blind carbon copy (BCC) recipient to the email message.
        /// </summary>
        /// <param name="address">The recipient's address.</param>
        /// <returns>This class.</returns>
        public EmailMessage Bcc(MailAddress address)
        {
            Mail.Bcc.Add(address);
            return this;
        }

        /// <summary>
        /// Adds an alternate form of the message body.
        /// </summary>
        /// <param name="item">The alternate view to add.</param>
        /// <returns>This class.</returns>
        public EmailMessage AddAlternateView(AlternateView item)
        {
            Mail.AlternateViews.Add(item);
            return this;
        }

        /// <summary>
        /// Adds an attachment to the email message.
        /// </summary>
        /// <param name="item">The attachment to add.</param>
        /// <returns>This class.</returns>
        public EmailMessage AddAttachment(Attachment item)
        {
            Mail.Attachments.Add(item);
            return this;
        }

        /// <summary><
        /// Sends the email.
        /// </summary>
        public virtual void Send()
        {
            FillDefaultAddress();
            using var mailClient = new SmtpClient(_settings.Value.MailServer);
            mailClient.Send(Mail);
        }

        /// <summary>
        /// Sends the email asynchronously.
        /// </summary>
        public virtual async Task SendAsync()
        {
            FillDefaultAddress();
            using var mailClient = new SmtpClient(_settings.Value.MailServer);
            await mailClient.SendMailAsync(Mail).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets To and From addresses to the admin if they have not been set.
        /// </summary>
        protected void FillDefaultAddress()
        {
            if (Mail.To.Count == 0)
            {
                Mail.To.Add(DefaultMailAddress);
            }
            if (Mail.From?.Address == null)
            {
                Mail.From = DefaultMailAddress;
            }
        }

        /// <summary>
        /// Returns the host's default email as defined in the configuration.
        /// </summary>
        public MailAddress DefaultMailAddress => new MailAddress(_settings.Value.MailFrom, _settings.Value.MailFromName);
    }
}
