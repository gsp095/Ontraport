using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Satrimono.Models;
using Satrimono.Pages;
using EmergenceGuardian.WebTools.UnitTests;
using Xunit;

namespace Satrimono.UnitTests
{
    public class BookAccuracyListDetailModelTests
    {
        private SatrimonoContext _db;
        private List<Book> _seedData;

        private BookAccuracyListDetailModel SetupModel(bool seed = false)
        {
            _db = new SatrimonoContext(InMemoryDbContext<SatrimonoContext>.GetTestDbOptions());
            if (seed)
            {
                _seedData = SatrimonoSeedData.AddBookSeedData(_db);
            }
            return new BookAccuracyListDetailModel(_db);
        }

        [Fact]
        public void Constructor_NullParam_ThrowsException()
        {
            Action act = () => new ContactModel(null);

            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void Constructor_Valid_NoException()
        {
            SetupModel();
        }

        [Theory]
        [InlineData("holy-bible-king-james")]
        [InlineData("the-alchemist")]
        public async Task OnGetAsync_ValidKey_HasBooksWithKey(string key)
        {
            var model = SetupModel(true);

            var result = await model.OnGetAsync(key);

            var expected = _seedData.Where(x => x.Key == key).Select(x => x.Id);
            Assert.Equal(expected, model.Books.Select(x => x.Id));
            Assert.Equal(expected.FirstOrDefault(), model.Books.Select(x => x.Id).FirstOrDefault());
        }

        [Theory]
        [InlineData("the-alchemist")]
        public async Task OnGetAsync_ValidKey_PageResult(string key)
        {
            var model = SetupModel(true);

            var result = await model.OnGetAsync(key);

            Assert.IsType<PageResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid")]
        public async Task OnGetAsync_KeyNotFound_RedirectToPageResult(string key)
        {
            var model = SetupModel(true);

            var result = await model.OnGetAsync(key);

            Assert.IsType<RedirectToPageResult>(result);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData(0, "Death")]
        [InlineData(10000, "Force of Life")]
        [InlineData(500, "Love")]
        [InlineData(-500, "Black Magic")]
        public void GetVibrationDescription_BookWithVibration_ReturnsDescription(int? vibration, string expected)
        {
            var model = SetupModel();
            var book = new Book()
            {
                Vibration = vibration
            };

            var result = model.GetVibrationDescription(book);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, null, false, "")]
        [InlineData(null, null, true, "Fiction book.")]
        [InlineData(419, .69, false, "Recommendation: AVOID")]
        [InlineData(900, null, false, "")]
        [InlineData(900, .8, false, "Near Enlightenment")]
        [InlineData(900, .9, false, "Near Enlightenment and highly accurate")]
        public void GetReadingOverview_BookWithVibrationAndAccuracy_ReturnsDescription(int? vibration, double? accuracy, bool isFiction, string expected)
        {
            var model = SetupModel();
            var book = new Book()
            {
                Vibration = vibration,
                Accuracy = accuracy,
                IsFiction = isFiction
            };

            var result = model.GetReadingOverview(book);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("http//mybook.com", false)]
        [InlineData("http://amzn.to/mybook", true)]
        [InlineData("mylink_amazon", true)]
        public void GetPurchaseLink_BookWithUrl_HtmlLink(string url, bool? expectedAmazon)
        {
            var model = SetupModel();
            var book = new Book()
            {
                Url = url
            };
            model.Books = new List<Book>() { book };

            var result = model.GetPurchaseLink(book);

            if (expectedAmazon == null)
            {
                Assert.Empty(result);
            }
            else
            {
                Assert.StartsWith("<a ", result);
                Assert.Contains(url, result);
                result.ContainsOrNot("View on Amazon", expectedAmazon.Value);
            }
        }

        [Theory]
        [InlineData(4)]
        [InlineData(10)]
        public void GetPurchaseLink_MoreThanThreeBooks_NoLink(int bookCount)
        {
            var model = SetupModel();
            var book = new Book()
            {
                Url = "http//mybook.com"
            };
            model.Books = new Book[bookCount];

            var result = model.GetPurchaseLink(book);

            Assert.Empty(result);
        }
    }
}
