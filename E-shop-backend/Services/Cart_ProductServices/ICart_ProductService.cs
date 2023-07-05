using E_shop_backend.Models;

namespace E_shop_backend.Services.Cart_ProductServices
{
    public interface ICart_ProductService
    {
        Cart_Product CreateCartProduct(Cart_Product cart_Product);
        Cart_Product UpdateCartProduct(Cart_Product cart_Product);
        Cart_Product DeleteCartProduct(Cart_Product cart_Product);
        bool CartProductExists(Cart_Product cart_Product);
    }
}
