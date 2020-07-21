using System;

namespace HanumanInstitute.CommonWeb.Email
{
    /// <summary>
    /// Creates new instances of EmailSender.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Creates a new instance of EmailSender to send an email.
        /// </summary>
        /// <returns>A new IEmailSender instance.</returns>
        IEmailMessage Create();
        /// <summary>
        /// Creates a new instance of EmailSender to send an email with specified subject and body.
        /// </summary>
        /// <param name="subject">The email's subject.</param>
        /// <param name="body">The email's body.</param>
        /// <returns>A new IEmailSender instance.</returns>
        IEmailMessage Create(string subject, string body);
    }
}
