using AutoMapper;
using Azure.Core;
using E_shop_backend.Dtos;
using E_shop_backend.Models;
using E_shop_backend.Services.Product_GenreService;
using E_shop_backend.Services.ProductServices;
using E_shop_backend.Services.ReviewService;
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
        private readonly IMapper _mapper;
        private readonly IProduct_GenreService _product_GenreService;
        private readonly IReviewService _reviewService;

        public ProductController(IProductService productService,
            IMapper mapper,
            IProduct_GenreService product_GenreService,
            IReviewService reviewService)
        {
            _productService = productService;
            _mapper = mapper;
            _product_GenreService = product_GenreService;
            _reviewService = reviewService;
        }

        [HttpGet("getProducts")]
        public IActionResult GetProducts()
        {
            try
            {
                var products = _productService.GetProducts();
                var productsMap = _mapper.Map<List<ResProductsDto>>(products);
                return Ok(productsMap);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("createProduct")]
        public IActionResult CreateProduct(ReqProductDto newProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Title = newProduct.Title,
                Description = newProduct.Description,
                ImageUrl = newProduct.ImageUrl,
                Seasons = newProduct.Seasons,
                Price = newProduct.Price,
                RatingId = newProduct.RatingId,
                StudioId = newProduct.StudioId,
            };
            var createdProduct = _productService.CreateProduct(product);
            // Iterate over ProductGenre array and create new ProductGenre for numbers in the array
            var productId = createdProduct.Id;
            foreach (var genreId in newProduct.ProductGenres)
            {
                var productGenre = new Product_Genre
                {
                    ProductId = productId,
                    GenreId = genreId
                };

                // Add the Product_Genre entity to the context
                _product_GenreService.CreateProduct_Genre(productGenre);
            }

            return Ok(createdProduct);
        }

        [HttpDelete("deleteProduct/{productId}")]
        public IActionResult DeleteProduct(int productId)
        {
            try
            {
                var res = _productService.DeleteProduct(productId);
                return Ok(res);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getSingleProduct/{productId}")]
        public IActionResult GetProduct(int productId)
        {
            // Get product itself
            var product = _productService.GetProductById(productId);
            // Get product genres
            product.ProductGenres = _product_GenreService.GetProduct_Genre(product.Id);
            // Get product reviews
            product.Reviews = _reviewService.GetProductReviews(product.Id);
            return Ok(product);
        }

        [HttpPut("updateProduct")]
        public IActionResult UpdateProduct(ResProductsDto updatedProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productMap = _mapper.Map<Product>(updatedProduct);
            try
            {
                var product = _productService.UpdateProduct(productMap);
                return Ok(product);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
