using E_shop_backend.Dtos;
using E_shop_backend.Models;

namespace E_shop_backend.Services.ProductServices
{
    public interface IProductService
    {
        Task<ServiceResponse<ICollection<ResProductDto>>> GetProducts(string? query);
        Task<ServiceResponse<SingleProductDto>> GetProductById(int Id);
        Task<ServiceResponse<Product>> CreateProduct(ReqProductDto product);
        Task<ServiceResponse<Product>> UpdateProduct(ReqProductDto product);
        Task<ServiceResponse<Product>> DeleteProduct(int Id);
    }
}
