using E_shop_backend.Data;
using E_shop_backend.Models;

namespace E_shop_backend.Services.Cart_ProductServices
{
    public class Cart_ProductService : ICart_ProductService
    {
        private readonly DataContext _context;

        public Cart_ProductService(DataContext context)
        {
            _context = context;
        }

        public Cart_Product CreateCartProduct(Cart_Product cart_Product)
        {
            _context.Cart_Products.Add(cart_Product);
            _context.SaveChanges();
            return cart_Product;
        }

        public Cart_Product DeleteCartProduct(Cart_Product cart_Product)
        {
            var userCart = _context.Cart_Products
                .FirstOrDefault(c => c.CartId == cart_Product.CartId && 
                c.ProductId == cart_Product.ProductId );

            if (userCart == null)
            {
                throw new Exception("Product doesnt exist");
            }

            _context.Cart_Products.Remove(userCart);
            _context.SaveChanges();
            return userCart;
        }

        public Cart_Product UpdateCartProduct(Cart_Product newCart_Product)
        {
            var cartProduct = _context.Cart_Products
                .FirstOrDefault(c => c.CartId == newCart_Product.CartId && 
                c.ProductId == newCart_Product.ProductId);

            if (cartProduct == null)
            {
                throw new Exception("Product doesnt exist");
            }
            // Assigning new data
            cartProduct.Seasons = newCart_Product.Seasons;

            _context.Cart_Products.Update(cartProduct);
            _context.SaveChanges();
            return cartProduct;
        }

        public bool CartProductExists(Cart_Product cart_Product)
        {
            return _context.Cart_Products.Any(u =>
                u.CartId == cart_Product.CartId &&
                u.ProductId == cart_Product.ProductId);
        }
    }
}
