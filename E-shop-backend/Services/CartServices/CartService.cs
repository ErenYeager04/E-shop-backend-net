using E_shop_backend.Data;
using E_shop_backend.Dtos;
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

        public async Task<ServiceResponse<Cart_Product>> AddProductToCart(Cart_ProductDto cart_Product)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<Cart_Product>();
            var result = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == cart_Product.UserId);
            // If user doesnt have cart create one and add the product
            if (result == null)
            {
                var newUserCart = new Cart { UserId = cart_Product.UserId };
                await _context.Carts.AddAsync(newUserCart);
                await _context.SaveChangesAsync();

                var newCartProduct = new Cart_Product
                {
                    CartId = newUserCart.Id,
                    ProductId = cart_Product.ProductId,
                    Seasons = cart_Product.Seasons,
                };
                await _context.Cart_Products.AddAsync(newCartProduct);
                await _context.SaveChangesAsync();
                serviceResponse.Success = true;
                serviceResponse.Data = newCartProduct;
                return serviceResponse;
            }

            var newUserCartProduct = new Cart_Product
            {
                CartId = result.Id,
                ProductId = cart_Product.ProductId,
                Seasons = cart_Product.Seasons
            };
            // Check if user already have the item
            var userHasProduct = await _context.Cart_Products.AnyAsync(u =>
                u.CartId == newUserCartProduct.CartId &&
                u.ProductId == newUserCartProduct.ProductId);
            if (userHasProduct == true)
            {
                serviceResponse.Message = "You already have this item in your cart";
                serviceResponse.Success = false;
                return serviceResponse;
            }
            await _context.Cart_Products.AddAsync(newUserCartProduct);
            await _context.SaveChangesAsync();
            serviceResponse.Success = true;
            serviceResponse.Data = newUserCartProduct;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Cart>> DeleteUserCart(int userId)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<Cart>();

            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                .SingleOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "You dont have a cart";
                return serviceResponse;
            }

            // Delete all associated CartProducts
            _context.Cart_Products.RemoveRange(cart.CartProducts);

            // Delete the Cart
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();
            serviceResponse.Success = true;
            serviceResponse.Data = cart;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Cart_Product>> DeleteProductFromCart(Cart_ProductDto cart_Product)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<Cart_Product>();
            // Get user cart ID
            var userCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == cart_Product.UserId);
            // Retrive product from the database
            var userCartProduct = await _context.Cart_Products
                .FirstOrDefaultAsync(c => c.CartId == userCart.Id &&
                c.ProductId == cart_Product.ProductId);

            if (userCartProduct == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Product doesnt exist";
                return serviceResponse;
            }
            // Remove the product from DB
            _context.Cart_Products.Remove(userCartProduct);
            await _context.SaveChangesAsync();
            serviceResponse.Success = true;
            serviceResponse.Data = userCartProduct;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Cart>> GetUserCart(int UserId)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<Cart>();
            var result = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == UserId);
            if (result == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "You have nothing inside the cart";
                return serviceResponse;
            }
 
            var response = await _context.Carts
                .Include(c => c.CartProducts)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == UserId);
            serviceResponse.Success = true;
            serviceResponse.Data = response;
            return serviceResponse;
        }
    }
}
