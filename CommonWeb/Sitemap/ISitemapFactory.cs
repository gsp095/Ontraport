using System;
using Microsoft.AspNetCore.Mvc;

namespace HanumanInstitute.CommonWeb.Sitemap
{
    /// <summary>
    /// Creates instances of ISitemapBuilder.
    /// </summary>
    public interface ISitemapFactory
    {
        /// <summary>
        /// Creates a new sitemap builder.
        /// </summary>
        ISitemapBuilder Create();
        /// <summary>
        /// Creates a new sitemap builder.
        /// </summary>
        ISitemapBuilder Create(IUrlHelper urlHelper);
    }
}
