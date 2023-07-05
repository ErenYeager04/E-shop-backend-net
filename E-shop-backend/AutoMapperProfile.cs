using AutoMapper;
using E_shop_backend.Dtos;
using E_shop_backend.Models;

namespace E_shop_backend
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<User, LoginDto>().ReverseMap();
            CreateMap<Product, ResProductsDto>();
            CreateMap<Product, ResProductsDto>().ReverseMap();
            CreateMap<Product, ReqProductDto>().ReverseMap();
            CreateMap<Review, ReviewDto>().ReverseMap();
        }
    }
}
