using System;

namespace HanumanInstitute.CommonWeb.Sitemap
{
    /// <summary>
    /// Represents how often a page is changed, for the XML sitemap.
    /// </summary>
    public enum ChangeFrequency
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }
}
