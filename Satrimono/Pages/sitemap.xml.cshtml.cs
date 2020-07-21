using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Sitemap;
using HanumanInstitute.Satrimono.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.Satrimono.Pages
{
    /// <summary>
    /// Contains the website sitemap information that can be used for XML sitemap, HTML sitemap, or navigation.
    /// </summary>
    public class SitemapModel : PageModel
    {
        private readonly ISitemapFactory _sitemapFactory;
        private readonly SatrimonoContext _db;

        public SitemapModel(ISitemapFactory sitemapFactory, SatrimonoContext dbContext)
        {
            _sitemapFactory = sitemapFactory.CheckNotNull(nameof(sitemapFactory));
            _db = dbContext.CheckNotNull(nameof(dbContext));
        }

        /// <summary>
        /// Returns the sitemap builder.
        /// </summary>
        [NotNull]
        public ISitemapBuilder? Builder { get; private set; }

        /// <summary>
        /// Fills the sitemap structure.
        /// </summary>
        public void OnGet()
        {
            Builder = _sitemapFactory.Create(Url);

            Builder.AddPage("book-accuracy-list", null, null, 0.5, "Book Accuracy List");
            Builder.AddPage("books", null, null, 0.1, "Books");
            Builder.AddPage("bulk-order", null, null, 0.1, "Bulk Order");
            Builder.AddPage("contact", null, null, 0.1, "Contact");
            Builder.AddPage("email-sent", null, null, 0.0);
            Builder.AddPage("error", null, null, 0.0);
            Builder.AddPage("how-to-measure-hawkins-scale", null, null, 1.0, "How to Measure Hawkins Scale");
            Builder.AddPage("how-to-muscle-testing", null, null, 1.0, "How to Do Muscle Testing");
            Builder.AddPage("how-to-pendulum-reading", null, null, 1.0, "How to Do Pendulum Reading");
            Builder.AddPage("index", null, null, 0.2, "Home");
            Builder.AddPage("mission", null, null, 0.1, "Mission");
            Builder.AddPage("privacy", null, null, 0.1, "Privacy");
            Builder.AddPage("sitemap", null, null, 0.1, "Sitemap");
            Builder.AddPage("terms", null, null, 0.1, "Terms");

            var query = from b in _db.Book
                        orderby b.Title
                        select new
                        {
                            b.Title,
                            b.Key
                        };
            var parent = Builder.AddUrl(null, null, null, null, "Books Reviews");
            foreach (var book in query.Distinct())
            {
                Builder.AddUrl(Url.AbsolutePage($"book-accuracy-list-detail", null, new { key = book.Key }), null, null, 0.6, $"{book.Title}", parent);
            }
        }
    }
}
