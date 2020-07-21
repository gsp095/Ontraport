using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Res = HanumanInstitute.CommonWebApp.Properties.Resources;

namespace HanumanInstitute.CommonWeb.Identity
{
    public class LogoutManager<T, TKey> : ILogoutManager where T : IdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        private readonly SignInManager<T> _signInManager;
        private readonly ILogger<LogoutManager<T, TKey>> _logger;

        public LogoutManager(SignInManager<T> signInManager, ILogger<LogoutManager<T, TKey>> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "Reviewed: Framework LocalRedirect uses a string")]
        public async Task<IActionResult> LogOutAsync(PageModel page, string returnUrl = "")
        {
            page.CheckNotNull(nameof(page));

            await _signInManager.SignOutAsync().ConfigureAwait(false);
            _logger.LogInformation(Res.LogoutManagerUserLoggedOut);
            if (returnUrl != null)
            {
                return page.LocalRedirect(returnUrl);
            }
            else
            {
                return page.RedirectToPage("/Index");
            }
        }
    }
}
