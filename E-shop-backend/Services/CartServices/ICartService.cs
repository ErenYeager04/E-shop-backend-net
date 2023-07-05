using E_shop_backend.Models;

namespace E_shop_backend.Services.CartServices
{
    public interface ICartService
    {
        Cart GetCart(int UserId);
        Cart CreateCart(Cart cart);
        Cart UserHaveCart(int userId);
        Cart DeleteUserCart(int cartId);
    }
}
