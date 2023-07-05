using AutoMapper;
using E_shop_backend.Dtos;
using E_shop_backend.Models;
using E_shop_backend.Services.Cart_ProductServices;
using E_shop_backend.Services.CartServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_shop_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICart_ProductService _cart_ProductService;
        private readonly IMapper _mapper;

        public CartController(ICartService cartService,
            ICart_ProductService cart_ProductService,
            IMapper mapper)
        {
            _cartService = cartService;
            _cart_ProductService = cart_ProductService;
            _mapper = mapper;
        }

        [HttpPost("addProduct")]
        public IActionResult createProduct(Cart_ProductDto cart_Product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _cartService.UserHaveCart(cart_Product.UserId);
            // If user doesnt have cart crete one and add the product
            if(result == null)
            {
                var newUserCart = new Cart { UserId =  cart_Product.UserId };
                var createdCart = _cartService.CreateCart(newUserCart);

                var newCartProduct = new Cart_Product
                {
                    CartId = createdCart.Id,
                    ProductId = cart_Product.ProductId,
                    Seasons = cart_Product.Seasons,
                };
                var response = _cart_ProductService.CreateCartProduct(newCartProduct);
                return Ok(response);
            }

            var newUserCartProduct = new Cart_Product
            {
                CartId = result.Id,
                ProductId = cart_Product.ProductId,
                Seasons = cart_Product.Seasons
            };
            // Check if user already have the item
            if(_cart_ProductService.CartProductExists(newUserCartProduct))
            {
                return BadRequest("You already have this item in your cart");
            }
            var addedProduct = _cart_ProductService.CreateCartProduct(newUserCartProduct);
            return Ok(addedProduct);
        }

        [HttpGet("getUserCart/{userId}")]
        public IActionResult GetUserCart(int userId)
        {
            var result = _cartService.UserHaveCart(userId);
            if(result == null)
            {
                return BadRequest("You have nothing inside the cart");
            }
            try
            {
                var userCart = _cartService.GetCart(userId);
                return Ok(userCart);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deleteProduct")]
        public IActionResult DeleteProductFromCart(Cart_ProductDto cart_Product)
        {
            // Get user cart ID
            var userCart = _cartService.UserHaveCart(cart_Product.UserId);
            var productToDelete = new Cart_Product
            {
                CartId = userCart.Id,
                ProductId = cart_Product.ProductId,
            };
            try
            {
                var response = _cart_ProductService.DeleteCartProduct(productToDelete);
                return Ok(response);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deleteCart/{userId}")]
        public IActionResult DeleteCart(int userId)
        {
            try
            {
                var userCart = _cartService.UserHaveCart(userId);
                var response = _cartService.DeleteUserCart(userCart.Id);
                return Ok(response);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updateProduct")]
        public IActionResult UpdateProduct(Cart_ProductDto cart_Product)
        {
            // Get user cart ID
            var userCart = _cartService.UserHaveCart(cart_Product.UserId);
            var newCartProduct = new Cart_Product
            {
                CartId = userCart.Id,
                ProductId = cart_Product.ProductId,
                Seasons = cart_Product.Seasons,
            };
            try
            {
                var result = _cart_ProductService.UpdateCartProduct(newCartProduct);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
