using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTO.Categories;
using ProductShop.DTO.CategoryProduct;
using ProductShop.DTO.Products;
using ProductShop.DTO.Users;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();


            //string pathUsers = File.ReadAllText("../../../Datasets/users.json");
            //string pathProducts = File.ReadAllText("../../../Datasets/products.json");
            //string pathCategories = File.ReadAllText("../../../Datasets/categories.json");
            //string pathCategoriesProducts = File.ReadAllText("../../../Datasets/categories-products.json");

            //ImportUsers(context, pathUsers);
            //ImportProducts(context, pathProducts);
            //ImportCategories(context, pathCategories);
            //var result = ImportCategoryProducts(context, pathCategoriesProducts);
            //Console.WriteLine(result);
            
            //Console.WriteLine(GetProductsInRange(context));
            //Console.WriteLine(GetSoldProducts(context));
            //Console.WriteLine(GetCategoriesByProductsCount(context));
            Console.WriteLine(GetUsersWithProducts(context));
        }
        //P09:
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users1 = context.Users
                .Include(x =>x.ProductsSold)
                .ToList()
                .Where(x => x.ProductsSold.Any(y => y.BuyerId != null))
                .Select(p => new
                {
                    firstName = p.FirstName,
                    lastName = p.LastName,
                    age = p.Age,
                    soldProducts = new
                    {
                        count = p.ProductsSold.Where(x => x.BuyerId != null).Count(),
                        products = p.ProductsSold
                        .Where(x => x.BuyerId != null)
                        .Select(a => new
                        {
                            name = a.Name,
                            price = a.Price
                        }).ToArray(),
                    }
                })
                .OrderByDescending(z =>z.soldProducts.products.Count())
                .ToList();

            var resultObject = new
            {
                usersCount = context.Users.Where(x => x.ProductsSold.Any(y => y.BuyerId != null)).Count(),
                users = users1
            };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var result = JsonConvert.SerializeObject(resultObject, Formatting.Indented, settings);
            return result;
        }

        //P08:
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count(),
                    averagePrice = x.CategoryProducts.Average(a => a.Product.Price).ToString("F2"),
                    totalRevenue = x.CategoryProducts.Sum(a =>a.Product.Price).ToString("F2")
                })
                .OrderByDescending(a =>a.productsCount)
                .ToList();
            var result = JsonConvert.SerializeObject(categories, Formatting.Indented);
            return result;
        }

        //P07:
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold
                    .Where(a =>a.BuyerId != null)
                    .Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    }).ToList()
                }).OrderBy(x => x.lastName)
                .ThenBy(a => a.firstName)
                .ToList();
            var result = JsonConvert.SerializeObject(users, Formatting.Indented);
            return result;
        }
      
        //P06:
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + " " + x.Seller.LastName
                })
                .OrderBy(x => x.price)
                .ToList();
            var result = JsonConvert.SerializeObject(products, Formatting.Indented);
            return result;
        }
        //P05:
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InicializeMapper();
            var dtoCategoryproduct = 
                JsonConvert.DeserializeObject<IEnumerable<CategoryproductInputModel>>(inputJson);
            var categoryproduct = mapper.Map<IEnumerable<CategoryProduct>>(dtoCategoryproduct);

            context.CategoryProducts.AddRange(categoryproduct);
            context.SaveChanges();

            return $"Successfully imported {categoryproduct.Count()}";
        }

        //P04:
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InicializeMapper();
            var dtoCategories = JsonConvert.DeserializeObject<IEnumerable<CategoriesInputModel>>(inputJson)
                .Where(x =>x.Name != null);

            var categories = mapper.Map<IEnumerable<Category>>(dtoCategories);
            context.Categories.AddRange(categories);
            context.SaveChanges();

            return  $"Successfully imported {categories.Count()}";
        }

        //P03:
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InicializeMapper();
            var dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductsInputModel>>(inputJson);

            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        //P02:
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InicializeMapper();
            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);

            var users = mapper.Map<IEnumerable<User>>(dtoUsers);
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }
        private static void InicializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            mapper = config.CreateMapper();
        }
       
    }
}