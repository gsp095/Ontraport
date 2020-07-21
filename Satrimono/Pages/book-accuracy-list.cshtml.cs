using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.Satrimono.Models;
using HanumanInstitute.CommonWeb;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HanumanInstitute.Satrimono.Pages
{
    public class BookAccuracyListModel : PageModel
    {
        public IEnumerable<Book>? Books { get; set; }
        public string Search { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string TitleSortParam { get; set; } = string.Empty;
        public string AuthorSortParam { get; set; } = string.Empty;
        public string AccuracySortParam { get; set; } = string.Empty;
        public string VibrationSortParam { get; set; } = string.Empty;
        private readonly SatrimonoContext _db;

        public BookAccuracyListModel(SatrimonoContext dbContext)
        {
            _db = dbContext.CheckNotNull(nameof(DbContext));
        }

        public async Task OnGetAsync(string sort, string search, string author)
        {
            TitleSortParam = string.IsNullOrEmpty(sort) ? "title_desc" : "";
            AuthorSortParam = sort == "author" ? "author_desc" : "author";
            AccuracySortParam = sort == "accuracy_desc" ? "accuracy" : "accuracy_desc";
            VibrationSortParam = sort == "vibration_desc" ? "vibration" : "vibration_desc";

            var books = from s in _db.Book
                        select s;

            if (!string.IsNullOrEmpty(search))
            {
                Search = search;
                var searchPattern = $"%{search}%";
                books = books.Where(b => EF.Functions.Like(b.Title, searchPattern)
                            || EF.Functions.Like(b.Subtitle, searchPattern)
                            || EF.Functions.Like(b.Author, searchPattern));
            }
            if (!string.IsNullOrEmpty(author))
            {
                Author = author;
                books = books.Where(b => b.Author == author);
            }

            books = sort switch
            {
                // Note: We sort last by Id instead of Subtitle for books that have several sections (title and author are equal for several entries)
                // Then the order of the sections (as entered in the database) is more relevant than the alphabetical order of the sections.
                "title_desc" => books.OrderByDescending(s => s.Title).ThenByDescending(s => s.Author).ThenByDescending(s => s.Id),
                "author" => books.OrderBy(s => s.Author).ThenBy(s => s.Title).ThenBy(s => s.Id),
                "author_desc" => books.OrderByDescending(s => s.Author).ThenByDescending(s => s.Title).ThenByDescending(s => s.Id),
                "vibration" => books.OrderBy(s => s.Vibration).ThenBy(s => s.Accuracy),
                "vibration_desc" => books.OrderByDescending(s => s.Vibration).ThenByDescending(s => s.Accuracy),
                "accuracy" => books.OrderBy(s => s.Accuracy).ThenBy(s => s.Vibration),
                "accuracy_desc" => books.OrderByDescending(s => s.Accuracy).ThenByDescending(s => s.Vibration),
                // "title"
                _ => books.OrderBy(s => s.Title).ThenBy(s => s.Author).ThenBy(s => s.Id),
            };
            Books = await books.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public static string GetBookTitle(Book item)
        {
            item.CheckNotNull(nameof(item));

            if (!string.IsNullOrEmpty(item.Subtitle))
            {
                return $"{item.Title}: {item.Subtitle}";
            }
            else
            {
                return item.Title;
            }
        }
    }
}
