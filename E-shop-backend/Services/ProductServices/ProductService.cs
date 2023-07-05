using E_shop_backend.Data;
using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace E_shop_backend.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }
        public Product CreateProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public Product DeleteProduct(int Id)
        {
            // Get product that we want to delete
            var product = _context.Products.Find(Id);

            if (product == null)
            {
                throw new Exception("Product doesnt exist");
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return product;
        }

        public Product GetProductById(int Id)
        {
            // Get product with studio table and rating table
            var product = _context.Products
                .Where(p => p.Id == Id)
                .Include(p => p.Studio)
                .Include(p => p.Rating)
                .FirstOrDefault();
            if (product == null)
            {
                throw new Exception("Product doesnt exist");
            }
            return product;
        }

        public ICollection<Product> GetProducts()
        {
            var response = _context.Products.OrderBy(p => p.Id).ToList();
            return response;
        }

        public Product UpdateProduct(Product product)
        {
            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);

            if (existingProduct == null)
            {
                throw new Exception("Product doesnt exist");
            }
            // Asigning product with the new data
            existingProduct.Title = product.Title;
            existingProduct.Description = product.Description;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.Seasons = product.Seasons;
            existingProduct.Price = product.Price;
            existingProduct.RatingId = product.RatingId;
            existingProduct.StudioId = product.StudioId;

            _context.Products.Update(existingProduct);
            _context.SaveChanges();
            return existingProduct;
        }
    }
}
