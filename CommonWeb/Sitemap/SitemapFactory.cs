using System;
using Microsoft.AspNetCore.Mvc;

namespace HanumanInstitute.CommonWeb.Sitemap
{
    /// <summary>
    /// Creates instances of ISitemapBuilder.
    /// </summary>
    public class SitemapFactory : ISitemapFactory
    {
        /// <summary>
        /// Creates a new sitemap builder.
        /// </summary>
        public ISitemapBuilder Create() => new SitemapBuilder(null);

        /// <summary>
        /// Creates a new sitemap builder.
        /// </summary>
        public ISitemapBuilder Create(IUrlHelper urlHelper) => new SitemapBuilder(urlHelper);
    }
}
