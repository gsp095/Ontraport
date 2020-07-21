using System;
using System.Collections.Generic;
using Satrimono.Models;

namespace Satrimono.UnitTests
{
    /// <summary>
    /// Provides seed data for unit tests.
    /// </summary>
    public static class SatrimonoSeedData
    {
        /// <summary>
        /// Returns seed data for the Book table.
        /// </summary>
        /// <param name="db">The context in which to insert seed data.</param>
        /// <returns>The seed data as a list.</returns>
        public static List<Book> AddBookSeedData(SatrimonoContext db)
        {
            var result = new List<Book>()
            {
                new Book()
                {
                    Id = 2,
                    Key = "holy-bible-king-james",
                    Title = "Holy Bible",
                    Subtitle = "Old Testament",
                    Author = "King James Version",
                    Accuracy = 0.93,
                    Vibration = 4418
                },
                new Book()
                {
                    Id = 3,
                    Key = "holy-bible-king-james",
                    Title = "Holy Bible",
                    Subtitle = "New Testament",
                    Author = "King James Version",
                    Accuracy = 0.895,
                    Vibration = 1018
                },
                new Book()
                {
                    Id = 29,
                    Key = "the-alchemist",
                    Title = "The Alchemist",
                    Author = "Paulo Coelho",
                    Accuracy = 0.813,
                    Vibration = 816,
                    IsFiction = true
                },
                new Book()
                {
                    Id = 58,
                    Key = "the-law-of-one",
                    Title = "The Law of One",
                    Subtitle = "Book Two",
                    Author = "Ra",
                    Accuracy = 0.939,
                    Vibration = 801
                },
                new Book()
                {
                    Id = 59,
                    Key = "the-law-of-one",
                    Title = "The Law of One",
                    Subtitle = "Book Three",
                    Author = "Ra",
                    Accuracy = 0.982,
                    Vibration = 814
                },
                new Book()
                {
                    Id = 60,
                    Key = "the-law-of-one",
                    Title = "The Law of One",
                    Subtitle = "Book Four",
                    Author = "Ra",
                    Accuracy = 0.927,
                    Vibration = 819
                }
            };
            if (db != null)
            {
                db.Book.AddRange(result);
                db.SaveChanges();
            }
            return result;
        }
    }
}
