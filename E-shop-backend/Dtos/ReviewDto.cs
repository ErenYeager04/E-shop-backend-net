using E_shop_backend.Models;
using System.Text.Json.Serialization;

namespace E_shop_backend.Dtos
{
    public class ReviewDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
