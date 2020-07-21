using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.RemoteHealingTech.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Res = HanumanInstitute.RemoteHealingTech.Properties.Resources;

namespace HanumanInstitute.RemoteHealingTech.Areas.Subscribe.Pages
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<RegisterModel> logger, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public string? ReturnUrl { get; set; }
        [BindProperty]
        public string Handler { get; set; }

        public RegisterInputModel RegisterInput { get; set; }
        public LoginInputModel LoginInput { get; set; }

        public class RegisterInputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
            [Required]
            [Display(Name = "User Role")]
            public string UserRole { get; set; } = string.Empty;
        }

        public class LoginInputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostRegisterAsync(RegisterInputModel registerInput = null)
        {
            ReturnUrl ??= Url.Content("~/");
            if (registerInput != null && ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = registerInput.Email, Email = registerInput.Email,UserRole = registerInput.UserRole };
                var result = await _userManager.CreateAsync(user, registerInput.Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(Res.LogNewAccount);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                    var callbackUrl = Url.Page(
                        "/Subscribe/Confirm-Email",
                        pageHandler: null,
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);

                    await _emailSender.Create(Res.ConfirmEmailTitle,
                        Res.ConfirmEmail.FormatInvariant(HtmlEncoder.Default.Encode(callbackUrl)))
                        .To(registerInput.Email).SendAsync().ConfigureAwait(false);

                    await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                    return LocalRedirect(ReturnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public async Task<IActionResult> OnPostLoginAsync(LoginInputModel loginInput = null)
        {
            ReturnUrl ??= Url.Content("~/");
            if (loginInput != null && ModelState.IsValid)
            {
                var _userDetails = await _userManager.FindByNameAsync(loginInput.Email).ConfigureAwait(false);
                if (_userDetails != null)
                {
                    var IsValidUser = await _signInManager.CheckPasswordSignInAsync(_userDetails, loginInput.Password, false).ConfigureAwait(false);
                    if (IsValidUser.Succeeded)
                    {
                        var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);

                        identity.AddClaim(new Claim(ClaimTypes.Name, _userDetails.UserName));
                        identity.AddClaim(new Claim(ClaimTypes.Role, _userDetails.UserRole));

                        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                            new ClaimsPrincipal(identity));

                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(ReturnUrl);


                    }
                    if (IsValidUser.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");

                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
                //var result = await _signInManager.PasswordSignInAsync(loginInput.Email, loginInput.Password, loginInput.RememberMe, lockoutOnFailure: false);
                //if (result.Succeeded)
                //{
                //    _logger.LogInformation("User logged in.");
                //    return LocalRedirect(ReturnUrl);
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out.");

                //    return RedirectToPage("./Lockout", new { Area = "Account" });
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return Page();
                //}


            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
