using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.RemoteHealingTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.RemoteHealingTech.Areas.Account.Pages
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSenderFactory;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSenderFactory)
        {
            _userManager = userManager;
            _emailSenderFactory = emailSenderFactory;
        }

        [BindProperty, ValidateObject]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/Reset-Password",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                await _emailSenderFactory.Create(
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.")
                    .To(Input.Email).SendAsync();

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
