using System.Text.Json.Serialization;

namespace E_shop_backend.Models
{
    public class Cart_Product
    {
        public int Id { get; set; }
        public int Seasons { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
