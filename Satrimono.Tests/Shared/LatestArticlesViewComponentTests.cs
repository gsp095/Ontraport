using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using EmergenceGuardian.WebCommon;
using Satrimono.Pages.Shared;
using Satrimono.Models;
using Xunit;
using Moq;
using System.Collections.Generic;
using System.Xml;

namespace Satrimono.UnitTests.Shared
{
    public class LatestArticlesViewComponentTests
    {
        private LatestArticlesConfig _config;
        private Mock<ISyndicationFeedService> _rssMock;
        private const string _validUri = "valid";
        private const string _invalidUri = "invalid";

        private _LatestArticlesViewComponent SetupModel()
        {
            _config = new LatestArticlesConfig();
            _rssMock = new Mock<ISyndicationFeedService>();
            var rssItems = new List<SyndicationItem>();
            for (var i=0; i<10; i++)
            {
                rssItems.Add(new SyndicationItem()
                {
                    Title = new TextSyndicationContent($"title{i}"),
                    Summary = new TextSyndicationContent(new string(i.ToString()[0], 500)),
                    BaseUri = new Uri("http://uri.com")
                });
            }
            _rssMock.Setup(x => x.Load(_validUri)).Returns(new SyndicationFeed(rssItems));
            _rssMock.Setup(x => x.Load(_invalidUri)).Callback(() => throw new XmlException());
            return new _LatestArticlesViewComponent(_config, _rssMock.Object);
        }

        [Fact]
        public void Constructor_NullParam_ThrowsException()
        {
            void act() => new _LatestArticlesViewComponent(null, null);

            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void Constructor_Valid_NoException()
        {
            SetupModel();
        }

        [Fact]
        public async Task InvokeAsync_ValidUrl_ReturnsView()
        {
            var model = SetupModel();
            _config.ArticlesRss = _validUri;

            var result = await model.InvokeAsync();

            Assert.IsType<ViewViewComponentResult>(result);
        }

        [Fact]
        public async Task InvokeAsync_InvalidUrl_ReturnsEmptyContent()
        {
            var model = SetupModel();
            _config.ArticlesRss = _invalidUri;

            var result = await model.InvokeAsync();

            Assert.IsType<ContentViewComponentResult>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public async Task InvokeAsync_ListLength_ListWithSpecifiedLength(int listLength)
        {
            var model = SetupModel();
            _config.ArticlesRss = _validUri;
            _config.ListLength = listLength;

            var result = await model.InvokeAsync();
            var resultList = GetResultModel(result).ToList();

            Assert.Equal(listLength, resultList.Count());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public async Task InvokeAsync_MaxTextLength_SummaryWithSpecifiedLength(int maxTextLength)
        {
            var model = SetupModel();
            _config.ArticlesRss = _validUri;
            var hellipLength = "&hellip;".Length;
            _config.ListLength = 1;
            _config.MaxTextLength = maxTextLength;

            var result = await model.InvokeAsync();
            var resultList = GetResultModel(result);

            Assert.Equal(maxTextLength + hellipLength, resultList.First().Summary.Length);
        }

        private IEnumerable<_LatestArticlesViewComponent.RssItem> GetResultModel(IViewComponentResult result)
        {
            return (result as ViewViewComponentResult).ViewData.Model as IEnumerable<_LatestArticlesViewComponent.RssItem>;
        }
    }
}
