using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace E_shop_backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Cart_Product> Cart_Products { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Product_Genre> Product_Genres { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Studio> Studios { get; set; }
    }
}
