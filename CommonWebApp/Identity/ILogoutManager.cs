using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.CommonWeb.Identity
{
    public interface ILogoutManager
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "Reviewed: Framework LocalRedirect uses a string")]
        Task<IActionResult> LogOutAsync(PageModel page, string returnUrl = "");
    }
}
