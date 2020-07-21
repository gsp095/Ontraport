using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HanumanInstitute.Satrimono.Models;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.Satrimono.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.Satrimono.Components
{
    public class RssLatest : ViewComponent
    {
        private readonly ISyndicationFeedService _rss;
        private readonly IOptions<LatestArticlesConfig> _config;

        public RssLatest(ISyndicationFeedService rss, IOptions<LatestArticlesConfig> config)
        {
            _rss = rss.CheckNotNull(nameof(rss));
            _config = config.CheckNotNull(nameof(config));
        }

        public async Task<IViewComponentResult> InvokeAsync(int feedIndex)
        {
            try
            {
                var conf = _config.Value;
                var feedUrl = feedIndex == 0 ? conf.ArticlesRss : conf.FederationOfLightRss;
                var feed = await Task.Run(() => _rss.Load(feedUrl!)).ConfigureAwait(false);
                var items = feed.Items.Take(conf.ListLength).ToList().Select(x => new RssItem()
                {
                    Uri = x.Links.FirstOrDefault()?.Uri,
                    Title = x.Title?.Text ?? string.Empty,
                    Summary = TrimText(HtmlToPlainText(x.Summary?.Text ?? string.Empty), conf.MaxTextLength)
                });
                return View(items);
            }
            catch
            {
                return Content(string.Empty);
            }
        }

        private static string TrimText(string title, int length)
        {
            var result = title;
            if (title != null && title.Length > length)
            {
                result = title.Substring(0, length).Trim();
                result += "&hellip;";
            }
            return result;
        }

        private static string HtmlToPlainText(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }
            else
            {
                return new Regex("<[^>]*>", RegexOptions.Multiline).Replace(html, string.Empty);
            }
        }
    }
}
