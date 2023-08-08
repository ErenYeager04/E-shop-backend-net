using E_shop_backend.Data;
using E_shop_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_shop_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private readonly DataContext _context;

        public HelperController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("getGenres")]
        public ICollection<Genre> GetGenres()
        {
            var response = _context.Genres.OrderBy(g => g.Id).ToList();
            return response;
        }

        [HttpGet("getRatings")]
        public ICollection<Rating> GetRatings()
        {
            var response = _context.Ratings.OrderBy(g => g.Id).ToList();
            return response;
        }

        [HttpGet("getStudios")]
        public ICollection<Studio> GetStudios()
        {
            var response = _context.Studios.OrderBy(g => g.Id).ToList();
            return response;
        }
    }
}
