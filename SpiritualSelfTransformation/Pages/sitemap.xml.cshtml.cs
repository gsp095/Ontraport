using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Sitemap;
using System.Diagnostics.CodeAnalysis;

namespace HanumanInstitute.SpiritualSelfTransformation.Pages
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

            Builder.AddPage("/about-us", null, null, 0.1, "About Us");

            Builder.AddPage("/contact-us", null, null, 0.1, "Contact Us");

            var defEn = Builder.AddPage("/definitions/index", null, null, 0.5, "Definitions (EN)");
            Builder.AddPage("/definitions/what-is-alchemy", null, null, 0.5, "Alchemy", defEn);
            Builder.AddPage("/definitions/what-are-chakras", null, null, 0.5, "Chakras", defEn);
            Builder.AddPage("/definitions/what-are-emotions", null, null, 0.5, "Emotions", defEn);
            Builder.AddPage("/definitions/what-is-spirituality", null, null, 0.5, "Spirituality", defEn);

            var defEs = Builder.AddPage("/definitions/es/index", null, null, 0.5, "Definiciones (ES)");
            Builder.AddPage("/definitions/es/que-es-alquimia", null, null, 0.5, "Alquimia", defEs);
            Builder.AddPage("/definitions/es/que-son-emociones", null, null, 0.5, "Emociones", defEs);
            Builder.AddPage("/definitions/es/que-es-espiritualidad", null, null, 0.5, "Espiritualidad", defEs);

            var defFr = Builder.AddPage("/definitions/fr/index", null, null, 0.5, "Définitions (FR)");
            Builder.AddPage("/definitions/fr/alchimie", null, null, 0.5, "Alchimie", defFr);
            Builder.AddPage("/definitions/fr/emotions", null, null, 0.5, "Émotions", defFr);
            Builder.AddPage("/definitions/fr/spiritualite", null, null, 0.5, "Spiritualité", defFr);

            Builder.AddPage("/free-training", null, null, 0.2, "Free Training");

            Builder.AddPage("/index", null, null, 0.8, "Home");

            Builder.AddPage("/privacy", null, null, 0.1, "Privacy");

            var services = Builder.AddPage("/services", null, null, 0.1, "Services");
            Builder.AddPage("/strategy-session", null, null, 0.6, "Strategy Session", services);
            Builder.AddPage("/god-connection", null, null, 0.6, "God Connection Attunement", services);
            Builder.AddPage("/soul-alignment-reading", null, null, 0.6, "Soul Alignment Reading", services);
            Builder.AddPage("/crystal-attunement", null, null, 0.6, "Crystal Attunement", services);
            Builder.AddPage("/powerliminals", null, null, 0.4, "Powerliminals", services);
            Builder.AddPage("/powerliminals-nonrivalry", null, null, 0.4, "Powerliminals Nonrivalry", services);
            Builder.AddPage("/remote-cell-reprogramming", null, null, 0.6, "Remote Cell Reprogramming", services);
            Builder.AddContent("/files/god-connection-attunement.pdf", null, null, 0.6, "How to Connect to God The Easy Way", services);
            Builder.AddPage("/alchemy-rings", null, null, 0.9, "Gemstones & Alchemy Rings", services);
            Builder.AddPage("/alchemy-rings-custom-men", null, null, 0.9, "Buy Custom Rings for Men", services);
            Builder.AddPage("/alchemy-rings-custom-women", null, null, 0.9, "Buy Custom Rings for Women", services);

            Builder.AddPage("/sitemap", null, null, 0.1, "Sitemap");

            Builder.AddPage("/terms", null, null, 0.1, "Terms");
        }
    }
}
