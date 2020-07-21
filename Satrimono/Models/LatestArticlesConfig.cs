using System;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.Satrimono.Models
{
    public class LatestArticlesConfig
    {
        [Required]
        public Uri? ArticlesRss { get; set; }
        [Required]
        public Uri? FederationOfLightRss { get; set; }
        [Range(0, 100)]
        public int ListLength { get; set; } = 4;
        [Range(0, int.MaxValue)]
        public int MaxTextLength { get; set; } = 128;
    }
}
