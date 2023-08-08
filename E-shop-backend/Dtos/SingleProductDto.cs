using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_shop_backend.Dtos
{
    public class SingleProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Seasons { get; set; }
        public decimal Price { get; set; }

        // Relations
        public ICollection<Genre> ProductGenres { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = null!;
        public Rating Rating { get; set; } = null!;
        public Studio Studio { get; set; } = null!;
    }
}
