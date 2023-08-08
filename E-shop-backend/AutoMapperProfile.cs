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
            CreateMap<Product, ResProductDto>();
            //CreateMap<Product, ResProductDto>().ReverseMap();
            CreateMap<Product, ReqProductDto>().ReverseMap();
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<Product, SingleProductDto>()
            .ForMember(dest => dest.ProductGenres, opt => opt.MapFrom(src => src.ProductGenres.Select(pg => new Genre { Id = pg.Genre.Id, Name = pg.Genre.Name }).ToList()))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews.Select(review => new Review { Rating = review.Rating, Comment = review.Comment, User = new User { FirstName = review.User.FirstName, LastName = review.User.LastName } }).ToList()));
        }
    }
}
