using System;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.CommonWeb.Email
{
    /// <summary>
    /// Contains configuration settings for sending emails.
    /// </summary>
    public class EmailConfig
    {
        /// <summary>
        /// Gets or sets the server to use for sending emails.
        /// </summary>
        [Required, DataType(DataType.Url)]
        public string MailServer { get; set; } = "localhost";
        /// <summary>
        /// Gets or sets the email address to send from.
        /// </summary>
        [Required, DataType(DataType.EmailAddress)]
        public string MailFrom { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name to display as the sender.
        /// </summary>
        public string MailFromName { get; set; } = string.Empty;
    }
}
