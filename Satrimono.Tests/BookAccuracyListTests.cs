using System;
using System.Linq;
using System.Threading.Tasks;
using EmergenceGuardian.WebTools.UnitTests;
using Satrimono.Models;
using Satrimono.Pages;
using Xunit;
using System.Collections.Generic;

namespace Satrimono.UnitTests
{
    public class BookAccuracyListModelTests
    {
        private SatrimonoContext _db;
        private List<Book> _seedData;

        private BookAccuracyListModel SetupModel()
        {
            _db = new SatrimonoContext(InMemoryDbContext<SatrimonoContext>.GetTestDbOptions());
            _seedData = SatrimonoSeedData.AddBookSeedData(_db);
            return new BookAccuracyListModel(_db);
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
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("invalid", "", "")]
        public async Task OnGetAsync_ArgsNullOrInvalid_BooksOrderedByTitleAuthorId(string sort, string search, string author)
        {
            var model = SetupModel();

            await model.OnGetAsync(sort, search, author);

            var expected = _seedData.OrderBy(x => x.Title).ThenBy(x => x.Author).ThenBy(x => x.Id).Select(x => x.Id);
            Assert.Equal(expected, model.Books.Select(x => x.Id));
        }

        [Fact]
        public async Task OnGetAsync_SortAuthorDesc_BooksOrderedByAuthorTitleIdDesc()
        {
            var model = SetupModel();

            await model.OnGetAsync("author_desc", null, null);

            var expected = _seedData.OrderByDescending(x => x.Author).ThenByDescending(x => x.Title).ThenByDescending(x => x.Id).Select(x => x.Id);
            Assert.Equal(expected, model.Books.Select(x => x.Id));
            Assert.Empty(model.TitleSortParam);
            Assert.Equal("author", model.AuthorSortParam);
            Assert.Equal("accuracy_desc", model.AccuracySortParam);
            Assert.Equal("vibration_desc", model.VibrationSortParam);
        }

        [Fact]
        public async Task OnGetAsync_SortAccuracy_BooksOrderedByAccuracyVibration()
        {
            var model = SetupModel();

            await model.OnGetAsync("accuracy", null, null);

            var expected = _seedData.OrderBy(x => x.Accuracy).ThenBy(x => x.Vibration).Select(x => x.Id);
            Assert.Equal(expected, model.Books.Select(x => x.Id));
            Assert.Empty(model.TitleSortParam);
            Assert.Equal("author", model.AuthorSortParam);
            Assert.Equal("accuracy_desc", model.AccuracySortParam);
            Assert.Equal("vibration_desc", model.VibrationSortParam);
        }

        [Theory]
        [InlineData("R")]
        [InlineData("bib")]
        [InlineData("king")]
        public async Task OnGetAsync_SearchValue_BooksFilteredByValue(string search)
        {
            var model = SetupModel();

            await model.OnGetAsync(null, search, null);

            var expected = _seedData.Where(x => x.Title?.Contains(search, StringComparison.CurrentCultureIgnoreCase) == true || x.Subtitle?.Contains(search, StringComparison.CurrentCultureIgnoreCase) == true || x.Author?.Contains(search, StringComparison.CurrentCultureIgnoreCase) == true)
                .OrderBy(x => x.Title).ThenBy(x => x.Author).ThenBy(x => x.Id).Select(x => x.Id);
            Assert.Equal(expected, model.Books.Select(x => x.Id));
            Assert.Equal(search, model.Search);
        }

        [Theory]
        [InlineData("R")]
        [InlineData("Ra")]
        [InlineData("ra")]
        [InlineData("King James Version")]
        public async Task OnGetAsync_AuthorValue_BooksFilteredByAuthor(string author)
        {
            var model = SetupModel();

            await model.OnGetAsync(null, null, author);

            var expected = _seedData.Where(x => x.Author == author)
                .OrderBy(x => x.Title).ThenBy(x => x.Author).ThenBy(x => x.Id).Select(x => x.Id);
            Assert.Equal(expected, model.Books.Select(x => x.Id));
            Assert.Equal(author, model.Author);
        }
    }
}
