using E_shop_backend.Dtos;
using E_shop_backend.Models;

namespace E_shop_backend.Services.CartServices
{
    public interface ICartService
    {
        Task<ServiceResponse<Cart>> GetUserCart(int UserId);
        Task<ServiceResponse<Cart>> DeleteUserCart(int userId);
        Task<ServiceResponse<Cart_Product>> DeleteProductFromCart(Cart_ProductDto cart_Product);
        Task<ServiceResponse<Cart_Product>> AddProductToCart(Cart_ProductDto cart_Product);
    }
}
