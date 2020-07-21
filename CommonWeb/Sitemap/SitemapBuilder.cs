using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HanumanInstitute.CommonWeb.Sitemap
{
    /// <summary>
    /// Facilitates the generation of an XML sitemap.
    /// </summary>
    public class SitemapBuilder : ISitemapBuilder
    {
        private readonly XNamespace _ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private readonly IUrlHelper? _urlHelper;

        /// <summary>
        /// Gets or sets the list of URLs to include in the sitemap.
        /// </summary>
        public List<SitemapUrl> Urls { get; private set; } = new List<SitemapUrl>();

        public SitemapBuilder(IUrlHelper? urlHelper)
        {
            _urlHelper = urlHelper;
        }

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
        //public SitemapUrl AddUrl(string url, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null) =>
        //    AddUrl(!string.IsNullOrEmpty(url) ? new Uri(url) : null, modified, changeFrequency, priority, displayTitle, parent);

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
        public SitemapUrl AddUrl(Uri? url, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null)
        {
            CheckPriority(priority);

            var item = new SitemapUrl()
            {
                Url = url,
                Modified = modified,
                ChangeFrequency = changeFrequency,
                Priority = priority,
                DisplayTitle = displayTitle,
                Parent = parent
            };
            Urls.Add(item);
            return item;
        }

        /// <summary>
        /// Adds a header to the sitemap that can be generated in HTML.
        /// </summary>
        /// <param name="displayTitle">The display title of the page.</param>
        /// <param name="parent">The parent SitemapUrl to create a hierarchical structure.</param>
        /// <returns>The SitemapUrl that got created.</returns>
        public SitemapUrl AddHeader(string displayTitle, SitemapUrl? parent = null) =>
            AddUrl(null, displayTitle: displayTitle, parent: parent);

        /// <summary>
        /// Adds a page to the sitemap.
        /// </summary>
        /// <param name="pageName">The page name.</param>
        /// <param name="modified">The last modification date of the page.</param>
        /// <param name="changeFrequency">How often the page is updated.</param>
        /// <param name="priority">The SEO priority of the page, between 0 and 10.</param>
        /// <param name="displayTitle">The display title of the page. If null, it can be excluded from the HTML sitemap.</param>
        /// <param name="parent">The parent SitemapUrl to create a hierarchical structure.</param>
        /// <returns>The SitemapUrl that got created.</returns>
        public SitemapUrl AddPage(string pageName, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null) =>
            AddUrl(_urlHelper!.AbsolutePage(pageName), modified, changeFrequency, priority, displayTitle, parent);

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
        public SitemapUrl AddContent(string contentPath, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null) =>
            AddUrl(_urlHelper!.AbsoluteContent(contentPath), modified, changeFrequency, priority, displayTitle, parent);

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
        public SitemapUrl AddAction(string actionName, string controllerName, object? routeValues = null, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, string displayTitle = "", SitemapUrl? parent = null) =>
            AddUrl(_urlHelper!.AbsoluteAction(actionName, controllerName, routeValues), modified, changeFrequency, priority, displayTitle, parent);

        /// <summary>
        /// Generates an XML sitemap based on the list of URLs.
        /// </summary>
        /// <returns>The XML sitemap</returns>
        public override string ToString()
        {
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(_ns + "urlset",
                    from item in Urls
                    where item.Url != null
                    select CreateItemElement(item)
                    ));

            return sitemap.ToString(SaveOptions.None);
        }


        /// <summary>
        /// Generates an XML node for specified SitemapUrl.
        /// </summary>
        /// <param name="url">A SitemapUrl with the details of a URL.</param>
        /// <returns>A XElement node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Reviewed: Sitemap standard is lowercase")]
        private XElement CreateItemElement(SitemapUrl url)
        {
            url.Url.CheckNotNull(nameof(url.Url));

            CheckPriority(url.Priority);

            var itemElement = new XElement(_ns + "url", new XElement(_ns + "loc", url.Url?.ToStringInvariant()));

            if (url.Modified.HasValue)
            {
                itemElement.Add(new XElement(_ns + "lastmod", url.Modified.Value.ToString("yyyy-MM-ddTHH:mm:ss.f", CultureInfo.InvariantCulture) + "+00:00"));
            }

            if (url.ChangeFrequency.HasValue)
            {
                itemElement.Add(new XElement(_ns + "changefreq", url.ChangeFrequency.Value.ToString().ToLowerInvariant()));
            }

            if (url.Priority.HasValue)
            {
                itemElement.Add(new XElement(_ns + "priority", url.Priority.Value.ToString("N1", CultureInfo.InvariantCulture)));
            }

            return itemElement;
        }

        private static void CheckPriority(double? priority)
        {
            if (priority != null && !(priority >= 0.0 && priority <= 10.0))
            {
                throw new ArgumentOutOfRangeException(Properties.Resources.SitemapPriorityOutOfRange);
            }
        }
    }
}
