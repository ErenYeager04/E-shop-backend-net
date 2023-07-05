using E_shop_backend.Data;
using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace E_shop_backend.Services.Product_GenreService
{
    public class Product_GenreService : IProduct_GenreService
    {
        private readonly DataContext _context;

        public Product_GenreService(DataContext context)
        {
            _context = context;
        }

        public Product_Genre CreateProduct_Genre(Product_Genre product_Genre)
        {
            _context.Product_Genres.Add(product_Genre);
            _context.SaveChanges();
            return product_Genre;
        }

        public Product_Genre DeleteProduct_Genre(Product_Genre product_Genre)
        {
            _context.Product_Genres.Remove(product_Genre);
            _context.SaveChanges();
            return product_Genre;
        }

        public ICollection<Product_Genre> GetProduct_Genre(int ProductId)
        {
            var productGenres = _context.Product_Genres
                .Where(p => p.ProductId == ProductId)
                .Include(p => p.Genre)
                .ToList();
            return productGenres;
        }

        public bool Product_GenreExists(Product_Genre product_Genre)
        {
            return _context.Product_Genres.Any(p => p == product_Genre);
        }

        public Product_Genre UpdateProduct_Genre(Product_Genre product_Genre)
        {
            _context.Product_Genres.Update(product_Genre);
            _context.SaveChanges();
            return product_Genre;
        }
    }
}
