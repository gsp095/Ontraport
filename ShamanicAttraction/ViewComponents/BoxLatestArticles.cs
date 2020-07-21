using System;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.ShamanicAttraction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.ShamanicAttraction.ViewComponents
{
    public class BoxLatestArticles : ViewComponent
    {
        private readonly IOptions<LatestArticlesConfig> _config;
        private readonly ISyndicationFeedService _rss;

        public BoxLatestArticles(IOptions<LatestArticlesConfig> config, ISyndicationFeedService rss)
        {
            _rss = rss ?? throw new ArgumentNullException(nameof(rss));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var feed = await Task.Run(() => _rss.Load(_config.Value.ArticlesRss)).ConfigureAwait(false);
                var items = feed.Items.Take(_config.Value.ListLength).ToList().Select(x => new RssItem()
                {
                    Uri = x.Links?.FirstOrDefault()?.Uri,
                    Title = x.Title?.Text ?? string.Empty
                }); ;
                return View(items);
            }
            catch
            {
                return Content(string.Empty);
            }
        }
    }
}
