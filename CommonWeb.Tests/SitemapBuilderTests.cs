using System;
using System.Globalization;
using HanumanInstitute.CommonWeb.Sitemap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Xunit;

namespace HanumanInstitute.CommonWeb.Tests
{
    public class SitemapBuilderTests
    {
        private static Uri ValidUrl => new Uri("http://www.abc.com");

        public IUrlHelper Url => _url ??= CreateUrl();
        private IUrlHelper? _url;
        private static IUrlHelper CreateUrl()
        {
            var helper = new Mock<IUrlHelper>();
            helper.Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>())).Returns<UrlRouteContext>(x => x.RouteName);
            return helper.Object;
        }

        public SitemapBuilder Builder => _builder ??= new SitemapBuilder(Url);
        private SitemapBuilder? _builder;

        [Fact]
        public void Constructor_HasEmptyList()
        {
            Assert.NotNull(Builder.Urls);
            Assert.Empty(Builder.Urls);
        }

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("", "2018-01-01", ChangeFrequency.Hourly, 0, "")]
        [InlineData("https://www.abc.com", "2020-10-10", ChangeFrequency.Always, 10, "Title")]
        public void AddUrl_Values_UrlsContainsValues(string url, string modified, ChangeFrequency? changeFrequency, double? priority, string displayTitle)
        {
            var modifiedDate = modified != null ? DateTime.Parse(modified, CultureInfo.InvariantCulture) : (DateTime?)null;

            var urlObj = url.HasValue() ? new Uri(url) : null;
            Builder.AddUrl(urlObj, modifiedDate, changeFrequency, priority, displayTitle, null);

            Assert.Single(Builder.Urls);
            var item = Builder.Urls[0];
            Assert.Equal(urlObj, item.Url);
            Assert.Equal(modifiedDate, item.Modified);
            Assert.Equal(changeFrequency, item.ChangeFrequency);
            Assert.Equal(priority, item.Priority);
            Assert.Equal(displayTitle, item.DisplayTitle);
            Assert.Null(item.Parent);
        }

        [Theory]
        [InlineData("2020-10-10", ChangeFrequency.Always, 10)]
        public void ToString_Values_StringContainsValues(string modified, ChangeFrequency? changeFrequency, double? priority)
        {
            var modifiedDate = modified != null ? DateTime.Parse(modified, CultureInfo.InvariantCulture) : (DateTime?)null;

            Builder.AddUrl(ValidUrl, modifiedDate, changeFrequency, priority);
            var result = Builder.ToString();

            Assert.Contains(ValidUrl.AbsoluteUri, result, StringComparison.InvariantCulture);
            Assert.Contains(modified, result, StringComparison.InvariantCulture);
            Assert.Contains(changeFrequency.ToString(), result, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains(priority.ToString(), result, StringComparison.InvariantCulture);
        }

        [Fact]
        public void ToString_OnlyUrl_StringDoesNotContainOtherValues()
        {
            Builder.AddUrl(ValidUrl);
            var result = Builder.ToString();

            Assert.DoesNotContain("lastmod", result, StringComparison.InvariantCulture);
            Assert.DoesNotContain("changefreq", result, StringComparison.InvariantCulture);
            Assert.DoesNotContain("priority", result, StringComparison.InvariantCulture);
        }

        [Fact]
        public void ToString_NullUrl_EmptySitemap()
        {
            Builder.AddUrl(null);
            var result = Builder.ToString();

            Assert.DoesNotContain("<url>", result, StringComparison.InvariantCulture);
        }

        [Fact]
        public void ToString_EmptyList_EmptySitemap()
        {
            var result = Builder.ToString();

            Assert.DoesNotContain("<url>", result, StringComparison.InvariantCulture);
        }

        [Theory]
        [InlineData(-.1)]
        [InlineData(11)]
        [InlineData(double.NaN)]
        public void AddUrl_InvalidPriority_ThrowsArgumentException(double? priority)
        {
            void Act() => Builder.AddUrl(ValidUrl, null, null, priority);

            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }
    }
}
