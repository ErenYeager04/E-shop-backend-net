using E_shop_backend.Models;

namespace E_shop_backend.Services.ProductServices
{
    public interface IProductService
    {
        ICollection<Product> GetProducts();
        Product GetProductById(int Id);
        Product CreateProduct(Product product);
        Product UpdateProduct(Product product);
        Product DeleteProduct(int Id);
    }
}
