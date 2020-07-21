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
    public class ContactModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            public string Name { get; set; } = string.Empty;
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; } = string.Empty;
            [Required]
            [DataType(DataType.MultilineText)]
            public string Message { get; set; } = string.Empty;
            [Required]
            public string Challenge { get; set; } = string.Empty;
        }

        private readonly IEmailSender _emailService;

        public ContactModel(IEmailSender emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (Input.Challenge == "four")
                {
                    var sender = _emailService.Create("Contact Us", CreateEmail()).ReplyTo(new MailAddress(Input.Email, Input.Name));
                    await sender.SendAsync().ConfigureAwait(false);
                    return RedirectToPage("/email-sent");
                }
                else
                {
                    ModelState.AddModelError("Input.Challenge", "Wrong challenge response");
                }
            }
            return Page();
        }

        private string CreateEmail()
        {
            var msg = new StringBuilder();
            msg.Append("Satrimono Publishing - Contact Us");
            msg.Append("\r\nName : ");
            msg.Append(Input.Name);
            msg.Append("\r\nEmail: ");
            msg.Append(Input.Email);
            msg.Append("\r\n\r\n");
            msg.Append(Input.Message);
            return msg.ToString();
        }
    }
}
