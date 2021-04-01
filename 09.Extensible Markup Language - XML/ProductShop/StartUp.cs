using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using XmlFacade;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string usersPath = File.ReadAllText("../../../Datasets/users.xml");
            //string productsPath = File.ReadAllText("../../../Datasets/products.xml");
            //string categoriesPath = File.ReadAllText("../../../Datasets/categories.xml");
            //string categoryProductsPath = File.ReadAllText("../../../Datasets/categories-products.xml");

            //ImportUsers(context, usersPath);
            //ImportProducts(context, productsPath);
            //ImportCategories(context, categoriesPath);
            //var result = ImportCategoryProducts(context, categoryProductsPath);
            //Console.WriteLine(result);
            Console.WriteLine(GetUsersWithProducts(context));
        }
        //P08:
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var allUsers = new AllUsersOutputModel
            {
                Count = context.Users.Where(x => x.ProductsSold.Any(p => p.Buyer != null)).Count(),
                Users = context.Users
              .ToList()
              .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
              .Select(u => new UsersproductsOutputModel
              {
                  FirstName = u.FirstName,
                  LastName = u.LastName,
                  Age = u.Age,
                  SoldProducts = new SoldProdOutputModel
                  {
                      Count = u.ProductsSold.Where(ps => ps.Buyer != null).Count(),
                      Products = u.ProductsSold.Where(ps => ps.Buyer != null)
                      .Select(p => new ProcutsSoldOutputModel
                      {
                          Name = p.Name,
                          Price = p.Price
                      })
                      .OrderByDescending(x => x.Price)
                      .ToArray()
                  }
              })
              .OrderByDescending(x => x.SoldProducts.Count)
              .Take(10)
              .ToArray()
            };
            var result = XmlConverter.Serialize(allUsers, "");
            return result;
        
        }
        //P07:
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(x => new CateforiesOutputModel
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Select(y => y.Product.Price).Average(),
                    TotalRevenue = x.CategoryProducts.Select(y => y.Product.Price).Sum()
                }).OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToList();

            var result = XmlConverter.Serialize(categories, "Categories");
            return result;
        }
        //P06:
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(s => s.BuyerId != null))
                
                .Select(x => new SoldOutputModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(p => new ProcutsSoldOutputModel
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToList();

            var result = XmlConverter.Serialize(users, "Users");
            return result;
        }
        //P05:
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ProductOutputModel
                {
                    Name = x.Name,
                    Price = x.Price,
                    BuyerFullName = x.Buyer.FirstName + " " + x.Buyer.LastName
                }).OrderBy(x => x.Price)
                .Take(10)
                .ToList();

            var result = XmlConverter.Serialize(products, "Products");
            return result;
        }
        //P04:
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var categoryIds = context.Categories.Select(x => x.Id).ToList();
            var productIds = context.Products.Select(x => x.Id).ToList();

            var dtoCategoryProducts = XmlConverter.Deserializer<CategoryProductsInputModel>(inputXml, "CategoryProducts");
            var categoryProducts = dtoCategoryProducts
                .Where(x => categoryIds.Contains(x.CategoryId) && productIds.Contains(x.ProductId))
                .Select(x => new CategoryProduct
                {
                    CategoryId = x.CategoryId,
                    ProductId = x.ProductId
                }).ToList();

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        //P03:
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var dtoCategories = XmlConverter.Deserializer<categoriesInputModel>(inputXml, "Categories");
            var categories = dtoCategories.Select(x => new Category
            {
                Name = x.Name
            }).ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }
        //P02:
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var dtoProducts = XmlConverter.Deserializer<ProductsInputModel>(inputXml, "Products");
            var products = dtoProducts.Select(x => new Product
            {
                Name = x.Name,
                Price = x.Price,
                SellerId = x.SellerId,
                BuyerId = x.BuyerId
            }).ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //P01:
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var dtoUsers = XmlConverter.Deserializer<UserInputModel>(inputXml, "Users");
            var users = dtoUsers.Select(x => new User
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Age = x.Age
            }).ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
    }
}