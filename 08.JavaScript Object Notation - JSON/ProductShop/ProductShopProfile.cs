using AutoMapper;
using ProductShop.DTO.Categories;
using ProductShop.DTO.CategoryProduct;
using ProductShop.DTO.Products;
using ProductShop.DTO.Users;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserInputModel, User>();
            this.CreateMap<ProductsInputModel, Product>();
            this.CreateMap<CategoriesInputModel, Category>();
            this.CreateMap<CategoryproductInputModel, CategoryProduct>();
        }
    }
}
