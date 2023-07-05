using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_shop_backend.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, MinLength(3), MaxLength(15)]
        public string Title { get; set; } = string.Empty;
        [Required, MinLength(3), MaxLength(15)]
        public string Description { get; set; } = string.Empty;
        [Required, Url]
        public string ImageUrl { get; set; } = string.Empty;
        public int Seasons { get; set; }
        [Required]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }

        // Relations
        public ICollection<Product_Genre> ProductGenres { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Cart_Product> CartProducts { get; set; } = null!;
        public int RatingId { get; set; }
        public Rating Rating { get; set; } = null!;
        public int StudioId { get; set; }
        public Studio Studio { get; set; } = null!;

    }
}
