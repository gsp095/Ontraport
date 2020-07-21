using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.CommonWeb.Tests
{
    /// <summary>
    /// Exposes extension methods to the PageModel.
    /// </summary>
    public static class PageModelExtensions
    {
        /// <summary>
        /// Marks a page's ModelState as invalid.
        /// </summary>
        /// <param name="page">The page to invalidate.</param>
        public static void SetModelStateInvalid(this PageModel page) => page.CheckNotNull(nameof(page)).ModelState.AddModelError("test", "test");
    }
}
