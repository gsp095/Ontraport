using System;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.ShamanicAttraction.Models
{
    /// <summary>
    /// Contains configuration about latest articles RSS feed.
    /// </summary>
    public class LatestArticlesConfig
    {
        [Required]
        public string ArticlesRss { get; set; } = string.Empty;
        [Range(0, 100)]
        public int ListLength { get; set; } = 10;
    }
}
