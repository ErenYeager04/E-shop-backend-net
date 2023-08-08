using AutoMapper;
using E_shop_backend.Data;
using E_shop_backend.Dtos;
using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace E_shop_backend.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReviewService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<Review>> CreateReview(ReviewDto Review)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<Review>();

            var reviewMap = _mapper.Map<Review>(Review);
            // Get user reviews
            var user = await _context.Users.Include(u => u.Reviews).FirstOrDefaultAsync(u => u.Id == reviewMap.UserId);
            if (user == null)
            {
                serviceResponse.Message = "User doesnt exist";
                serviceResponse.Success = false;
                return serviceResponse;
            }
            if(!_context.Products.Any(p => p.Id == reviewMap.ProductId))
            {
                serviceResponse.Message = "Product doesnt exist";
                serviceResponse.Success = false;
                return serviceResponse;
            }
            // If user already have review for the product send an exception
            if (user.Reviews.Any(u => u.ProductId == reviewMap.ProductId))
            {
                serviceResponse.Message = "You alredy have the review for the product";
                serviceResponse.Success = false;
                return serviceResponse;
            }

            try
            {
                await _context.Reviews.AddAsync(reviewMap);
                await _context.SaveChangesAsync();
                serviceResponse.Data = reviewMap;
                serviceResponse.Success = true;
                return serviceResponse;
            }catch(Exception ex)
            {
                serviceResponse.Message = "Error occured while saving the review";
                serviceResponse.Success = false;
                return serviceResponse;
            }
            
        }

    }
}
