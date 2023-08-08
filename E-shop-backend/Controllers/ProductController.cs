using Azure.Core;
using E_shop_backend.Dtos;
using E_shop_backend.Models;
using E_shop_backend.Services.ProductServices;
using E_shop_backend.Services.ReviewService;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_shop_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;

        public ProductController(IProductService productService,
            IReviewService reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        [HttpGet("getProducts")]
        public async Task<IActionResult> GetProducts([FromQuery] string? query)
        {

            // Getting the products
            var result = await _productService.GetProducts(query);
            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPost("createProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(ReqProductDto newProduct, [FromServices] IValidator<ReqProductDto> validator)
        {
            ValidationResult validationResult = validator.Validate(newProduct);

            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(errorMessage);
            }
            // Creating the product
            var result = await _productService.CreateProduct(newProduct);
            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpDelete("deleteProduct/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            // Deleting product
            var result = await _productService.DeleteProduct(productId);
            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);

        }

        [HttpGet("getSingleProduct/{productId}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            // Get product itself
            var result = await _productService.GetProductById(productId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPut("updateProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(ReqProductDto updatedProduct, [FromServices] IValidator<ReqProductDto> validator)
        {
            ValidationResult validationResult = validator.Validate(updatedProduct);

            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(errorMessage);
            };
            // Update the product
            var result = await _productService.UpdateProduct(updatedProduct);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
