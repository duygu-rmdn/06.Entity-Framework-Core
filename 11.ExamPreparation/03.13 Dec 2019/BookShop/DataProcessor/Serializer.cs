namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var books = context
                .Authors
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.AuthorsBooks
                        .OrderByDescending(p => p.Book.Price)
                        .Select(b => new
                        {
                            BookName = b.Book.Name,
                            BookPrice = b.Book.Price.ToString("F2")
                        })
                        .ToArray()
                })
                .ToArray()
                .OrderByDescending(b => b.Books.Length)
                .ToArray();

            return JsonConvert.SerializeObject(books, Formatting.Indented);
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var data = context.Books.ToList()
                .Where(x => x.PublishedOn < date && x.Genre == Genre.Science).OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Select(x => new BooksOutputModel
                {
                    Pages = x.Pages,
                    Name = x.Name,
                    Date = x.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                }).ToList()
                .Take(10)
                
                .ToList();
            var result = XmlConverter.Serialize(data, "Books");
            return result;
        }
    }
}/* var books = context
                .Authors
                .Select(a => new 
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.AuthorsBooks
                        .OrderByDescending(p => p.Book.Price)
                        .Select(b => new
                        {
                            BookName = b.Book.Name,
                            BookPrice = b.Book.Price.ToString("F2")
                        })
                        .ToArray()
                })
                .ToArray()
                .OrderByDescending(b => b.Books.Length)
                .ToArray();

            return JsonConvert.SerializeObject(books, Formatting.Indented);
*/
