using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HanumanInstitute.CommonWeb.Email
{
    /// <summary>
    /// Provides email sending service based on application configuration. To and From will be sent by default to the server host if not set.
    /// Properties can be set using fluent API.
    /// </summary>
    public interface IEmailMessage
    {
        /// <summary>
        /// Returns the MailMessage instance of the email to send.
        /// </summary>
        MailMessage Mail { get; }
        /// <summary>
        /// Sets the email subject and body.
        /// </summary>
        /// <param name="subject">The email's subject.</param>
        /// <param name="body">The email's body.</param>
        /// <returns>This class.</returns>
        EmailMessage SetMessage(string subject, string body);
        /// <summary>
        /// Adds a destination for this email message.
        /// </summary>
        /// <param name="address">The recipient's email address.</param>
        /// <param name="name">The recipient's name.</param>
        /// <returns>This class.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Reviewed: Framework uses MailMessage.To")]
        EmailMessage To(string address, string name = "");
        /// <summary>
        /// Adds a destination for this email message.
        /// </summary>
        /// <param name="address">The recipient's email address.</param>
        /// <returns>This class.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Reviewed: Framework uses MailMessage.To")]
        EmailMessage To(MailAddress address);
        /// <summary>
        /// Sets the sender of the email message.
        /// </summary>
        /// <param name="email">The sender's email address.</param>
        /// <param name="name">The sender's name.</param>
        /// <returns>This class.</returns>
        EmailMessage From(string email, string name = "");
        /// <summary>
        /// Sets the sender of the email message.
        /// </summary>
        /// <param name="address">The sender's email address.</param>
        /// <returns>This class.</returns>
        EmailMessage From(MailAddress address);
        /// <summary>
        /// Adds a reply to address of this message.
        /// </summary>
        /// <param name="address">The address to reply to.</param>
        /// <param name="name">The address name to reply to.</param>
        /// <returns>This class.</returns>
        EmailMessage ReplyTo(string address, string name = "");
        /// <summary>
        /// Adds a reply to address to this message.
        /// </summary>
        /// <param name="address">The address to reply to.</param>
        /// <returns>This class.</returns>
        EmailMessage ReplyTo(MailAddress address);
        /// <summary>
        /// Adds a carbon copy (CC) recipient to the email message.
        /// </summary>
        /// <param name="address">The recipient's address.</param>
        /// <param name="name">The recipient's name.</param>
        /// <returns>This class.</returns>
        EmailMessage CC(string address, string name = "");
        /// <summary>
        /// Adds a carbon copy (CC) recipient to the email message.
        /// </summary>
        /// <param name="address">The recipient's address.</param>
        /// <returns>This class.</returns>
        EmailMessage CC(MailAddress address);
        /// <summary>
        /// Adds a blind carbon copy (BCC) recipient to the email message.
        /// </summary>
        /// <param name="address">The recipient's address.</param>
        /// <param name="name">The recipient's name.</param>
        /// <returns>This class.</returns>
        EmailMessage Bcc(string address, string name = "");
        /// <summary>
        /// Adds a blind carbon copy (BCC) recipient to the email message.
        /// </summary>
        /// <param name="address">The recipient's address.</param>
        /// <returns>This class.</returns>
        EmailMessage Bcc(MailAddress address);
        /// <summary>
        /// Adds an alternate form of the message body.
        /// </summary>
        /// <param name="item">The alternate view to add.</param>
        /// <returns>This class.</returns>
        EmailMessage AddAlternateView(AlternateView item);
        /// <summary>
        /// Adds an attachment to the email message.
        /// </summary>
        /// <param name="item">The attachment to add.</param>
        /// <returns>This class.</returns>
        EmailMessage AddAttachment(Attachment item);
        /// <summary><
        /// Sends the email.
        /// </summary>
        void Send();
        /// <summary>
        /// Sends the email asynchronously.
        /// </summary>
        Task SendAsync();

        public MailAddress DefaultMailAddress { get; }
    }
}
