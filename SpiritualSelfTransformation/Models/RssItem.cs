using System;

namespace HanumanInstitute.SpiritualSelfTransformation.Models
{
    /// <summary>
    /// Represents a link retrieved from a RSS feed.
    /// </summary>
    public class RssItem
    {
        /// <summary>
        /// Gets or sets the Url of a page.
        /// </summary>
        public Uri? Uri { get; set; }
        /// <summary>
        /// Gets or sets the title of the page.
        /// </summary>
        public string Title { get; set; } = string.Empty;
    }
}
