namespace BookShop
{
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using Models.Enums;
    using System.Text;
    using System.Collections.Generic;
    using System.Globalization;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            //Console.WriteLine(GetBooksByAgeRestriction(db, "miNor"));
            //Console.WriteLine(GetGoldenBooks(db));
            //Console.WriteLine(GetBooksByPrice(db));
            //Console.WriteLine(GetBooksNotReleasedIn(db, 2000));
            //Console.WriteLine(GetBooksByCategory(db, "horror mystery drama"));
            //Console.WriteLine(GetBooksReleasedBefore(db, "30-12-1989"));
            //Console.WriteLine(GetAuthorNamesEndingIn(db, "dy"));
            //Console.WriteLine(GetBookTitlesContaining(db, "sK"));
            //Console.WriteLine(GetBooksByAuthor(db, "R"));
            //Console.WriteLine(CountBooks(db, 12));
            //Console.WriteLine(CountCopiesByAuthor(db));
            //Console.WriteLine(GetTotalProfitByCategory(db));
            //Console.WriteLine(GetMostRecentBooks(db));
            //IncreasePrices(db);
            RemoveBooks(db);

        }
        //P16:
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(c => c.Copies < 4200)
                .ToList();
            var booksCategory = context.BooksCategories
                .Where(c => c.Book.Copies < 4200)
                .ToList();

            context.BooksCategories.RemoveRange(booksCategory);
            context.Books.RemoveRange(books);

            context.SaveChanges();

            return books.Count();
        }

        //P15:
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in books)
            {
                book.Price += 5;
            }
            context.SaveChanges();
        }


        //P14:
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
               
               .Select(b => new
               {
                   Name = b.Name,
                   Book = b.CategoryBooks
                   .OrderByDescending(s =>s.Book.ReleaseDate)
                   .Take(3)
                   .Select(c => new
                   {
                       Title = c.Book.Title,
                       Release = c.Book.ReleaseDate
                   }).ToList()
               }).OrderBy(a =>a.Name)
               .ToList();
            var sb = new StringBuilder();
            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var item in category.Book)
                {
                    sb.AppendLine($"{item.Title} ({item.Release.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
        }

        //P13:
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profits = context.Categories
                .Select(bc => new
                {
                    Sum = bc.CategoryBooks
                    .Select(  z => new 
                    { 
                        sum1 = z.Book.Price * z.Book.Copies 
                    }).Sum(w => w.sum1),
                    Category = bc.Name
                })
                .OrderByDescending(c => c.Sum)
                .ThenBy(x => x.Category)
                .ToList();
            var sb = new StringBuilder();
            foreach (var profit in profits)
            {
                sb.AppendLine($"{profit.Category} ${profit.Sum:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        //12:
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var copies = context.Authors
                .Select(b => new
                {
                    Autor = $"{b.FirstName} {b.LastName}",
                    Copies = b.Books.Select(x => x.Copies).Sum()
                })
                .OrderByDescending(b => b.Copies)
                .ToList();

            var sb = new StringBuilder();
            foreach (var copy in copies)
            {
                sb.AppendLine($"{copy.Autor} - {copy.Copies}");
            }
            return sb.ToString().TrimEnd();
        }

        //11:
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .ToArray();

            return books.Length;
        }

        //P10:
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new 
                { 
                    b.Title, 
                    b.Author.FirstName,
                    b.Author.LastName,
                    b.BookId
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FirstName} {book.LastName})");
            }
            return sb.ToString().TrimEnd();
        }

        //P09:
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(x =>x)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }


        //P08:
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var autors = context.Authors
                .Where(b => b.FirstName.EndsWith(input))
                .Select(x => new { x.FirstName, x.LastName })
                .OrderBy(x => x.FirstName)
                .ThenBy(y => y.LastName)
                .ToList();

            var sb = new StringBuilder();
            foreach (var autor in autors)
            {
                sb.AppendLine($"{autor.FirstName} {autor.LastName}");
            }
            return sb.ToString().TrimEnd();
        }

        //P07:
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    x.Title,
                    Type = x.EditionType.ToString(),
                    x.Price
                })
                .ToList();
            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.Type} - ${book.Price:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        //P06:
        public static string GetBooksByCategory(BookShopContext context, string input)
        {

            string[] categories = input.ToLower()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            List<string> bookTitles = new List<string>();

            var books = context.Books
                .Select(b => new
                {
                    b.Title,
                    Categories = b.BookCategories.Select(bc => bc.Category.Name).ToList()
                })
                .ToList();

            foreach (var category in categories)
            {
                foreach (var book in books)
                {
                    if (book.Categories.Any(c => c.ToLower() == category))
                    {
                        bookTitles.Add(book.Title);
                    }
                }

            }

            bookTitles = bookTitles.OrderBy(t => t).ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var title in bookTitles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().TrimEnd();
        }

        //P05:
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(x => new { x.Title, x.BookId })
                .OrderBy(a => a.BookId)
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }
            return sb.ToString().TrimEnd();
        }

        //P04:
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(x => new { x.Price, x.Title })
                .OrderByDescending(x => x.Price)
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        //P03:
        public static string GetGoldenBooks(BookShopContext context)
        {
            
            var books = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .Select(b => new { b.Title, b.BookId })
                .OrderBy(x => x.BookId)
                .ToList();
            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }
            return sb.ToString().TrimEnd();
        }


        //P02:
        public static string GetBooksByAgeRestriction(BookShopContext context, string command) 
        {
            var lowerCom = command.ToLower();
            var books = context.Books
                .AsEnumerable()
                .Where(x => x.AgeRestriction.ToString().ToLower() == lowerCom)
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }
    }
}
