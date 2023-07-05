using E_shop_backend.Data;
using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace E_shop_backend.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;

        public CartService(DataContext context)
        {
            _context = context;
        }

        public Cart CreateCart(Cart cart)
        {
            _context.Carts.Add(cart);
            _context.SaveChanges();
            return cart;
        }

        public Cart DeleteUserCart(int cartId)
        {
            var cart = _context.Carts
                .Include(c => c.CartProducts)
                .SingleOrDefault(c => c.Id == cartId);

            // Delete all associated CartProducts
            _context.Cart_Products.RemoveRange(cart.CartProducts);

            // Delete the Cart
            _context.Carts.Remove(cart);

            _context.SaveChanges();
            return cart;
        }

        public Cart GetCart(int UserId)
        {
            var response = _context.Carts
                .Include(c => c.CartProducts)
                .ThenInclude(c => c.Product)
                .FirstOrDefault(c => c.UserId == UserId);
            return response;
        }

        public Cart UserHaveCart(int userId)
        {
            if(_context.Carts.Any(u => u.UserId == userId))
            {
                return _context.Carts.FirstOrDefault(c => c.UserId == userId);
            }
            else
            {
                return null;
            }
        }
    }
}
