using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace HanumanInstitute.ShamanicAttraction.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public string CodeName { get; set; } = "Error";
        public int? Code { get; set; }

        public void OnGet(int? code)
        {
            Code = code;
            if (code.HasValue)
            {
                CodeName = ReasonPhrases.GetReasonPhrase(code.Value);
            }
        }
    }
}
