using System.Text.Json.Serialization;

namespace E_shop_backend.Models
{
    public class Product_Genre
    {
        public int Id { get; set; }
        public int GenreId { get; set; }
        public int ProductId { get; set; }
        public Genre Genre { get; set; } = null!;
        [JsonIgnore]
        public Product Product { get; set; } = null!;
    }
}
