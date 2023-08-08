using E_shop_backend.Dtos;
using E_shop_backend.Models;
using E_shop_backend.Services.CartServices;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_shop_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("addProduct")]
        [Authorize]
        public async Task<IActionResult> createProduct(Cart_ProductDto cart_Product, [FromServices] IValidator<Cart_ProductDto> validator)
        {
            ValidationResult validationResult = validator.Validate(cart_Product);

            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(errorMessage);
            }
            // Adding the product to cart
            var result = await _cartService.AddProductToCart(cart_Product);

            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet("getUserCart/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserCart(int userId)
        {
            // Getting the user cart
            var result = await _cartService.GetUserCart(userId);

            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpDelete("deleteProduct")]
        [Authorize]
        public async Task<IActionResult> DeleteProductFromCart(Cart_ProductDto cart_Product)
        {
            // Adding the product to cart
            var result = await _cartService.DeleteProductFromCart(cart_Product);

            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpDelete("deleteCart/{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCart(int userId)
        {
            // Adding the product to cart
            var result = await _cartService.DeleteUserCart(userId);

            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
