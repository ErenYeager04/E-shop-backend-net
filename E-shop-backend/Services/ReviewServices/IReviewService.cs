using E_shop_backend.Models;

namespace E_shop_backend.Services.ReviewService
{
    public interface IReviewService
    {
        ICollection<Review> GetUserReviews(int userId);
        ICollection<Review> GetProductReviews(int productId);
        Review CreateReview(Review newReview);
    }
}
