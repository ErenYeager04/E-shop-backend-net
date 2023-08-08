using AutoMapper;
using Azure;
using E_shop_backend.Data;
using E_shop_backend.Dtos;
using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace E_shop_backend.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ProductService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Product>> CreateProduct(ReqProductDto product)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<Product>();
            // Making new Product object because Product_genre is just array of numbers
            var newProduct = new Product
            {
                Title = product.Title,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Seasons = product.Seasons,
                Price = product.Price,
                RatingId = product.RatingId,
                StudioId = product.StudioId,
            };
            // Adding new product to get the id from it
            try
            {
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();
            } catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Error occured while trying to save the product";
                return serviceResponse;
            }

            // Getting id from already created product
            var productId = newProduct.Id;
            foreach (var genreId in product.ProductGenres)
            {
                // Creating new Product_Genre for every number from Product_Genre array
                var productGenre = new Product_Genre
                {
                    ProductId = productId,
                    GenreId = genreId
                };

                // Add the Product_Genre entity to the context
                try
                {
                    await _context.Product_Genres.AddAsync(productGenre);
                } catch (Exception ex)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Error occured while trying to add genres to product";
                    return serviceResponse;
                }
            }
            await _context.SaveChangesAsync();
            serviceResponse.Success = true;
            serviceResponse.Data = newProduct;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Product>> DeleteProduct(int Id)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<Product>();
            // Get product that we want to delete
            var product = await _context.Products.FindAsync(Id);
            // If the product doesnt exist we send an error message
            if (product == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Product that you want to delete doesnt exist";
                return serviceResponse;
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                serviceResponse.Success = true;
                serviceResponse.Data = product;
                return serviceResponse;
            } catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Error occured while deleting the product";
                return serviceResponse;
            }
            
        }

        public async Task<ServiceResponse<SingleProductDto>> GetProductById(int Id)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<SingleProductDto>();
            try
            {
                // Get product with all the neccesary tables
                var product = await _context.Products
                .Where(p => p.Id == Id)
                .Include(p => p.ProductGenres)
                    .ThenInclude(p => p.Genre)
                .Include(p => p.Reviews)
                    .ThenInclude(p => p.User)
                .Include(p => p.Studio)
                .Include(p => p.Rating)
                .FirstOrDefaultAsync();
                if (product == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Product doesnt exist";
                    return serviceResponse;
                }
                // Map the product to  the dto
                var newProduct = _mapper.Map<SingleProductDto>(product);

                serviceResponse.Success = true;
                serviceResponse.Data = newProduct;
                return serviceResponse;
            } catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Error occured while retriving the product";
                return serviceResponse;
            }
            
        }

        public async Task<ServiceResponse<ICollection<ResProductDto>>> GetProducts(string? query)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<ICollection<ResProductDto>>();
            var productsQuery = _context.Products.AsQueryable();

            try
            {
                // Apply filters based on the query parameters
                if (!string.IsNullOrEmpty(query))
                {
                    productsQuery = productsQuery.Where(p => p.Title.Contains(query));
                }

                var products = await productsQuery.OrderBy(p => p.Id)
                    .Include(p => p.Reviews)
                    .ToListAsync();

                if(products.Count == 0)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "There is no such product";
                    return serviceResponse;
                }
                var productsMap = _mapper.Map<List<ResProductDto>>(products);

                foreach (var productMap in productsMap)
                {
                    int totalRating = 0;
                    int reviewCount = 0;

                    foreach (var review in productMap.Reviews)
                    {
                        totalRating += review.Rating;
                        reviewCount++;
                    }

                    if (reviewCount > 0)
                    {
                        double averageRating = (double)totalRating / reviewCount;
                        productMap.Rating = Math.Round(averageRating, 1);
                    }
                    else
                    {
                        productMap.Rating = 0;
                    }

                    // It's not recommended to clear the Reviews here, as it may cause unintended behavior in other parts of the code
                    productMap.Reviews.Clear();
                }

                serviceResponse.Data = productsMap;
                serviceResponse.Success = true;
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Error occurred while retrieving the products";
                return serviceResponse;
            }
                
        }
        public async Task<ServiceResponse<Product>> UpdateProduct(ReqProductDto product)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<Product>();
            var existingProduct = await _context.Products.Include(p => p.ProductGenres).FirstOrDefaultAsync(p => p.Id == product.Id);

            if (existingProduct == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Product wasnt found";
                return serviceResponse;
            }
            // Asigning product with the new data
            existingProduct.Title = product.Title;
            existingProduct.Description = product.Description;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.Seasons = product.Seasons;
            existingProduct.Price = product.Price;
            existingProduct.RatingId = product.RatingId;
            existingProduct.StudioId = product.StudioId;
            existingProduct.ProductGenres.Clear();
            foreach (var genreId in product.ProductGenres)
            {
                var productGenre = new Product_Genre
                {
                    ProductId = existingProduct.Id,
                    GenreId = genreId
                };
                existingProduct.ProductGenres.Add(productGenre);
            }
            try
            {
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();
                serviceResponse.Success = true;
                serviceResponse.Data = existingProduct;
                return serviceResponse;
            } catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Error occured while saving the update";
                return serviceResponse;
            }
            
        }
    }
}
