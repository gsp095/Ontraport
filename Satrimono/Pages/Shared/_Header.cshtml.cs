using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.Satrimono.Pages
{
    public class HeaderModel : PageModel
    {
        public int? ActiveIndex { get; set; }

        public HeaderModel() { }

        public HeaderModel(int? activeIndex)
        {
            ActiveIndex = activeIndex;
        }
    }
}
