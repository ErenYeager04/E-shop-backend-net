using System.Text.Json.Serialization;

namespace E_shop_backend.Models
{
    public class Genre
    { 
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public ICollection<Product_Genre> ProductGenres { get; set; } = null!;
    }
}
