using System;

namespace HanumanInstitute.SpiritualSelfTransformation.Models
{
    /// <summary>
    /// Contains strongly-typed custom properties for pages deriving from _Layout.
    /// </summary>
    public class LayoutData
    {
        /// <summary>
        /// Gets or sets the language of the page to write in the page header.
        /// </summary>
        public string Language { get; set; } = "en";
        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the page description.
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the page keywords.
        /// </summary>
        public string Keywords { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the page title for Facebook.
        /// </summary>
        public string FacebookTitle { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the page image for Facebook.
        /// </summary>
        public string FacebookImage { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets whether to display a Facebook like button.
        /// </summary>
        public bool FacebookLike { get; set; } = false;
        /// <summary>
        /// Gets or sets whether to display the sidebar.
        /// </summary>
        public bool SidebarVisible { get; set; } = true;
        /// <summary>
        /// Gets or sets whether to show the subscribe box at the top.
        /// </summary>
        public bool HeaderSubscribe { get; set; } = false;
        /// <summary>
        /// Gets or sets whether to enable PhotoSwipe on the page.
        /// </summary>
        public bool UsePhotoSwipe { get; set; } = false;
        /// <summary>
        /// Gets or sets whether to enable validation scripts on the page.
        /// </summary>
        public bool UseValidationScripts { get; set; } = false;
    }
}
