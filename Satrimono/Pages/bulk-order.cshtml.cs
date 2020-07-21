using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HanumanInstitute.CommonWeb.Email;
using System.Text;
using System.Net.Mail;

namespace HanumanInstitute.Satrimono.Pages
{
    [BindProperties]
    public class BulkOrderModel : PageModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Display(Name = "City / State / Province")]
        public string City { get; set; } = string.Empty;
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Book name")]
        public string BookName { get; set; } = "The History of the Universe";
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The quantity is invalid")]
        public int? Quantity { get; set; }
        [Required]
        [Display(Name = "Purpose for books")]
        public string Purpose { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Where did you hear about us")]
        public string Reference { get; set; } = string.Empty;
        [DataType(DataType.MultilineText)]
        [Display(Name = "Additional information")]
        public string AdditionalInformation { get; set; } = string.Empty;

        private readonly IEmailSender _emailService;

        public BulkOrderModel(IEmailSender emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _emailService
                .Create("Bulk Order", CreateEmail())
                .ReplyTo(new MailAddress(Email, Name))
                .SendAsync().ConfigureAwait(false);

            return RedirectToPage("/email-sent");
        }

        private string CreateEmail()
        {
            var msg = new StringBuilder();
            msg.Append("Satrimono Publishing - Bulk Order\r\n");
            msg.Append("\r\nName : ");
            msg.Append(Name);
            msg.Append("\r\nEmail: ");
            msg.Append(Email);
            msg.Append("\r\nCity/State/Province : ");
            msg.Append(City);
            msg.Append("\r\nCountry: ");
            msg.Append(Country);
            msg.Append("\r\nQty - The History of the Universe: ");
            msg.Append(BookName);
            msg.Append("\r\nPurpose for books: ");
            msg.Append(Purpose);
            msg.Append("\r\nWhere did you hear about us?: ");
            msg.Append(Reference);
            if (!string.IsNullOrEmpty(AdditionalInformation))
            {
                msg.Append("\r\n\r\nAdditional information: \r\n");
                msg.Append(AdditionalInformation);
            }
            return msg.ToString();
        }
    }
}
