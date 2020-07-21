using System;

namespace HanumanInstitute.CommonWeb.Sitemap
{
    /// <summary>
    /// Represents an item in the sitemap.
    /// </summary>
    public class SitemapUrl
    {
        /// <summary>
        /// Gets or sets the absolute URL of the page or action. If null, it will be excluded from the XML sitemap but can be displayed in an HTML sitemap.
        /// </summary>
        public Uri? Url { get; set; }
        /// <summary>
        /// Gets or sets the last modification date of the page.
        /// </summary>
        public DateTime? Modified { get; set; }
        /// <summary>
        /// Gets or sets how often the page is updated.
        /// </summary>
        public ChangeFrequency? ChangeFrequency { get; set; }
        /// <summary>
        /// Gets or sets the SEO priority of the page, between 0 and 10.
        /// </summary>
        public double? Priority { get; set; }
        /// <summary>
        /// Gets or sets the display title of the page. If null, it can be excluded from the HTML sitemap.
        /// </summary>
        public string DisplayTitle { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the parent SitemapUrl to create a hierarchical structures for display. Ignored for XML sitemap.
        /// </summary>
        public SitemapUrl? Parent { get; set; }

        /// <summary>
        /// Gets the depth of the parent hierarchy.
        /// </summary>
        /// <returns>0 if there is no parent, 1 if there is a parent, 2 if there is a parent with a parent, etc.</returns>
        public int ParentDepth
        {
            get
            {
                var depth = 0;
                var parent = this.Parent;
                while (parent != null)
                {
                    depth++;
                    parent = parent.Parent;
                }
                return depth;
            }
        }
    }
}
