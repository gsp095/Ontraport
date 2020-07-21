using System;
using System.Diagnostics.CodeAnalysis;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Sitemap;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.ShamanicAttraction.Pages
{
    /// <summary>
    /// Contains the website sitemap information that can be used for XML sitemap, HTML sitemap, or navigation.
    /// </summary>
    public class SitemapModel : PageModel
    {
        private readonly ISitemapFactory _sitemapFactory;

        public SitemapModel(ISitemapFactory sitemapFactory)
        {
            _sitemapFactory = sitemapFactory.CheckNotNull(nameof(sitemapFactory));
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

            var about = Builder.AddPage("/self-transformation/index", null, null, 0.1, "About Us");
            Builder.AddPage("/self-transformation/bio", null, null, 0.5, "Bio", about);
            Builder.AddPage("/self-transformation/contact-us", null, null, 0.1, "Contact Us", about);
            Builder.AddPage("/self-transformation/story", null, null, 0.1, "Etienne's Story", about);
            Builder.AddPage("/self-transformation/storydaniel", null, null, 0.1, "Story Daniel", about);
            Builder.AddPage("/self-transformation/storymonica", null, null, 0.1, "Story Monica", about);
            Builder.AddPage("/self-transformation/testimonials", null, null, 0.1, "Testimonials", about);
            Builder.AddPage("/self-transformation/what-we-do", null, null, 0.1, "What We Do", about);
            Builder.AddPage("/self-transformation/who-we-work-with", null, null, 0.1, "Who We Work With", about);


            var defEn = Builder.AddPage("/definitions/index", null, null, 0.5, "Definitions (EN)");
            Builder.AddPage("/definitions/what-is-animal-magnetism", null, null, 0.5, "Animal Magnetism", defEn);
            Builder.AddPage("/definitions/what-is-charisma", null, null, 0.5, "Charisma", defEn);
            Builder.AddPage("/definitions/what-is-sexual-attraction", null, null, 0.5, "Sexual Attraction", defEn);

            var defEs = Builder.AddPage("/definitions/es/index", null, null, 0.5, "Definiciones (ES)");
            Builder.AddPage("/definitions/es/que-es-atraccion-sexual", null, null, 0.5, "Atracción Sexual", defEs);
            Builder.AddPage("/definitions/es/que-es-carisma", null, null, 0.5, "Carisma", defEs);
            Builder.AddPage("/definitions/es/que-es-magnetismo-animal", null, null, 0.5, "Magnetismo Animal", defEs);

            var defFr = Builder.AddPage("/definitions/fr/index", null, null, 0.5, "Définitions (FR)");
            Builder.AddPage("/definitions/fr/attirance-sexuelle", null, null, 0.5, "Attirance Sexuelle", defFr);
            Builder.AddPage("/definitions/fr/charisme", null, null, 0.5, "Charisme", defFr);
            Builder.AddPage("/definitions/fr/magnetisme-animal", null, null, 0.5, "Magnetisme Animal", defFr);

            Builder.AddPage("/index", null, null, 0.8, "Home");

            Builder.AddPage("/privacy", null, null, 0.1, "Privacy");

            var products = Builder.AddPage("/products", null, null, 0.1, "Products");
            Builder.AddPage("/transform-from-within", null, null, 0.3, "Transform From Within to Attract Women", products);
            Builder.AddPage("/sexual-magnetism", null, null, 1.0, "Sexual Magnetism", products);
            Builder.AddPage("/result-oriented-spirituality", null, null, 0.6, "Result-Oriented Spirituality", products);

            Builder.AddPage("/recommended", null, null, 0.2, "Recommended Links");

            Builder.AddPage("/sitemap", null, null, 0.1, "Sitemap");

            Builder.AddPage("/terms", null, null, 0.1, "Terms");
        }
    }
}
