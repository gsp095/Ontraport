using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.SpiritualSelfTransformation.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public bool Share { get; set; }

        public void OnGet()
        {

        }
    }
}
