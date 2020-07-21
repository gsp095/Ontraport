using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.ShamanicAttraction.Pages
{
    public class EconfirmationModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Upgrade { get; set; } = string.Empty;

        public bool HasUpgrade => Upgrade == "xa";
    }
}
