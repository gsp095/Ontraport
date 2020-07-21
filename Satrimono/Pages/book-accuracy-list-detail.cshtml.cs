using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.Satrimono.Models;
using HanumanInstitute.CommonWeb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HanumanInstitute.Satrimono.Pages
{
    public class BookAccuracyListDetailModel : PageModel
    {
        public BookAccuracyListDetailModel(SatrimonoContext dbContext)
        {
            _db = dbContext;
        }

        private readonly SatrimonoContext _db;
        [NotNull]
        public IEnumerable<Book>? Books { get; set; }
        [NotNull]
        public Book? Book { get; set; }

        public async Task<IActionResult> OnGetAsync(string key)
        {
            var books = from s in _db.Book
                        where s.Key == key
                        select s;
            Books = await books.AsNoTracking().ToListAsync().ConfigureAwait(false);
            if (Books == null || !Books.Any())
            {
                return RedirectToPage("book-accuracy-list");
            }

            Book = Books.First();
            return Page();
        }

        public string GetVibrationDescription(Book item)
        {
            item.CheckNotNull(nameof(item));

            if (item.Vibration >= 8000)
            {
                return "Force of Life";
            }
            else if (item.Vibration >= 1000)
            {
                return "Enlightenment";
            }
            else if (item.Vibration >= 700)
            {
                return "Consciousness";
            }
            else if (item.Vibration >= 600)
            {
                return "Peace";
            }
            else if (item.Vibration >= 500)
            {
                return "Love";
            }
            else if (item.Vibration >= 400)
            {
                return "Reason";
            }
            else if (item.Vibration >= 350)
            {
                return "Acceptance";
            }
            else if (item.Vibration >= 310)
            {
                return "Willingness";
            }
            else if (item.Vibration >= 250)
            {
                return "Neutrality";
            }
            else if (item.Vibration >= 200)
            {
                return "Courage";
            }
            else if (item.Vibration >= 150)
            {
                return "Anger";
            }
            else if (item.Vibration >= 100)
            {
                return "Fear";
            }
            else if (item.Vibration >= 0)
            {
                return "Death";
            }
            else if (item.Vibration >= -450)
            {
                return "Corrupt Intent";
            }
            else if (item.Vibration < -450)
            {
                return "Black Magic";
            }
            else
            {
                return "";
            }
        }


        public string GetReadingOverview(Book item)
        {
            item.CheckNotNull(nameof(item));

            if (item.IsFiction == true)
            {
                if (item.Accuracy.HasValue)
                {
                    return "Fiction book. Resonance with truth: <strong>{0:p1}</strong>".FormatInvariant(item.Accuracy);
                }
                else
                {
                    return "Fiction book.";
                }
            }
            if (!item.Vibration.HasValue || !item.Accuracy.HasValue)
            {
                return "";
            }
            else if (item.Vibration < 400 || (item.Vibration < 420 && item.Accuracy < .7))
            {
                return "Recommendation: AVOID";
            }
            else if (item.Vibration < 420 && item.Accuracy >= .7)
            {
                return "Not very conscious but has good information";
            }
            else if (item.Accuracy < .5)
            {
                return "Severe distortion: has more disinformation than truth";
            }
            else if (item.Vibration < 650 && item.Accuracy < .7)
            {
                return "Not useful";
            }
            else if (item.Vibration < 650 && item.Accuracy >= .7)
            {
                return "Has good information";
            }
            else if (item.Vibration < 750 && item.Accuracy < .7)
            {
                return "Conscious, but read with a grain of salt";
            }
            else if (item.Vibration < 750 && item.Accuracy < .8)
            {
                return "Good book";
            }
            else if (item.Vibration < 750 && item.Accuracy < .9)
            {
                return "Good book with accurate information";
            }
            else if (item.Vibration < 750)
            {
                return "Good book with highly accurate information";
            }
            else if (item.Vibration < 850 && item.Accuracy < .7)
            {
                return "Highly conscious, but read with a grain of salt";
            }
            else if (item.Vibration < 850 && item.Accuracy < .8)
            {
                return "Great book";
            }
            else if (item.Vibration < 850 && item.Accuracy < .9)
            {
                return "Great book with accurate information";
            }
            else if (item.Vibration < 850)
            {
                return "Great book with highly accurate information";
            }
            else if (item.Vibration < 1000 && item.Accuracy < .9)
            {
                return "Near Enlightenment";
            }
            else if (item.Vibration < 1000)
            {
                return "Near Enlightenment and highly accurate";
            }
            else if (item.Vibration < 5000 && item.Accuracy < .9)
            {
                return "Enlightenment consciousness";
            }
            else if (item.Vibration < 5000)
            {
                return "Enlightenment consciousness and highly accurate";
            }
            else if (item.Vibration >= 5000)
            {
                return "Electrifying consciousness. MUST READ.";
            }
            else
            {
                return "";
            }
        }

        public string GetPurchaseLink(Book item)
        {
            item.CheckNotNull(nameof(item));

            if (string.IsNullOrEmpty(item.Url) || Books.Count() > 3)
            {
                return "";
            }
            else
            {
                return "<a href='{0}' target='_blank'>{1}</a>".FormatInvariant(
                    item.Url,
                    (item.Url.StartsWith("http://amzn.to", StringComparison.InvariantCultureIgnoreCase) || item.Url.Contains("amazon", StringComparison.InvariantCultureIgnoreCase)) ? "View on Amazon" : "View");
            }
        }
    }
}
