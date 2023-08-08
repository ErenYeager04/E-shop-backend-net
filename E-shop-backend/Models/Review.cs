using System.Text.Json.Serialization;

namespace E_shop_backend.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

    }
}
