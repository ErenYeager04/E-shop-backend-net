using E_shop_backend.Data;
using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace E_shop_backend.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly DataContext _context;

        public ReviewService(DataContext context)
        {
            _context = context;
        }

        public Review CreateReview(Review newReview)
        {
            // Get user reviews
            var user = _context.Users.Include(u => u.Reviews).FirstOrDefault(u => u.Id == newReview.UserId);
            if (user == null)
            {
                throw new Exception("User doesnt exist");
            }
            // If user already have review for the product throw exception
            if (user.Reviews.Any(u => u.ProductId == newReview.ProductId))
            {
                throw new Exception("You alredy have the review for the product");
            }
            _context.Reviews.Add(newReview);
            _context.SaveChanges();
            return newReview;
        }

        public ICollection<Review> GetProductReviews(int productId)
        {
            var reviews = _context.Reviews
            .Where(r => r.ProductId == productId)
            .ToList();

            return reviews;
        }

        public ICollection<Review> GetUserReviews(int userId)
        {
            var reviews = _context.Reviews
            .Where(r => r.UserId == userId)
            .ToList();

            return reviews;
        }
    }
}
