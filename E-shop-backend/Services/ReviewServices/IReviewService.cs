using E_shop_backend.Dtos;
using E_shop_backend.Models;

namespace E_shop_backend.Services.ReviewService
{
    public interface IReviewService
    {
        Task<ServiceResponse<Review>> CreateReview(ReviewDto newReview);
    }
}
