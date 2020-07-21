using System;
using System.Collections.Generic;

namespace HanumanInstitute.CommonWeb.Sitemap
{
    /// <summary>
    /// Facilitates the generation of an XML sitemap.
    /// </summary>
    public interface ISitemapBuilder
    {
        /// <summary>
        /// Gets or sets the list of URLs to include in the sitemap.
        /// </summary>
        List<SitemapUrl> Urls { get; }
        /// <summary>
        /// Adds an URL to the sitemap.
        /// </summary>
        /// <param name="url">The absolute URL of the page or action. If null, it will be excluded from the XML sitemap but can be displayed in an HTML sitemap.</param>
        /// <param name="modified">The last modification date of the page.</param>
        /// <param name="changeFrequency">How often the page is updated.</param>
        /// <param name="priority">The SEO priority of the page, between 0 and 10.</param>
        /// <param name="displayTitle">The display title of the page. If null, it can be excluded from the HTML sitemap.</param>
        /// <param name="parent">The parent SitemapUrl to create a hierarchical structure.</param>
        /// <returns>The SitemapUrl that got created.</returns>
        //SitemapUrl AddUrl(string url, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null);
        /// <summary>
        /// Adds a header to the sitemap that can be generated in HTML.
        /// </summary>
        /// <param name="displayTitle">The display title of the page.</param>
        /// <param name="parent">The parent SitemapUrl to create a hierarchical structure.</param>
        /// <returns>The SitemapUrl that got created.</returns>
        SitemapUrl AddHeader(string displayTitle, SitemapUrl? parent = null);
        /// <summary>
        /// Adds an URL to the sitemap.
        /// </summary>
        /// <param name="url">The absolute URL of the page or action. If null, it will be excluded from the XML sitemap but can be displayed in an HTML sitemap.</param>
        /// <param name="modified">The last modification date of the page.</param>
        /// <param name="changeFrequency">How often the page is updated.</param>
        /// <param name="priority">The SEO priority of the page, between 0 and 10.</param>
        /// <param name="displayTitle">The display title of the page. If null, it can be excluded from the HTML sitemap.</param>
        /// <param name="parent">The parent SitemapUrl to create a hierarchical structure.</param>
        /// <returns>The SitemapUrl that got created.</returns>
        SitemapUrl AddUrl(Uri? url, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null);
        /// <summary>
        /// Adds a page to the sitemap after converting it with Url.AbsolutePage.
        /// </summary>
        /// <param name="url">The page name.</param>
        /// <param name="modified">The last modification date of the page.</param>
        /// <param name="changeFrequency">How often the page is updated.</param>
        /// <param name="priority">The SEO priority of the page, between 0 and 10.</param>
        /// <param name="displayTitle">The display title of the page. If null, it can be excluded from the HTML sitemap.</param>
        /// <param name="parent">The parent SitemapUrl to create a hierarchical structure.</param>
        /// <returns>The SitemapUrl that got created.</returns>
        SitemapUrl AddPage(string pageName, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null);
        /// <summary>
        /// Adds a content path to the sitemap.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        /// <param name="modified">The last modification date of the page.</param>
        /// <param name="changeFrequency">How often the page is updated.</param>
        /// <param name="priority">The SEO priority of the page, between 0 and 10.</param>
        /// <param name="displayTitle">The display title of the page. If null, it can be excluded from the HTML sitemap.</param>
        /// <param name="parent">The parent SitemapUrl to create a hierarchical structure.</param>
        /// <returns>The SitemapUrl that got created.</returns>
        SitemapUrl AddContent(string contentPath, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null);
        /// <summary>
        /// Adds a content path to the sitemap.
        /// </summary>
        /// <param name="actionName">The content path.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="modified">The last modification date of the page.</param>
        /// <param name="changeFrequency">How often the page is updated.</param>
        /// <param name="priority">The SEO priority of the page, between 0 and 10.</param>
        /// <param name="displayTitle">The display title of the page. If null, it can be excluded from the HTML sitemap.</param>
        /// <param name="parent">The parent SitemapUrl to create a hierarchical structure.</param>
        /// <returns>The SitemapUrl that got created.</returns>
        SitemapUrl AddAction(string actionName, string controllerName, object? routeValues = null, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null);
        /// Generates an XML sitemap based on the list of URLs.
        /// </summary>
        /// <returns>The XML sitemap</returns>
        string ToString();
    }
}
