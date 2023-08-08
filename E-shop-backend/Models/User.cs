using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_shop_backend.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required, MinLength(3), MaxLength(15)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MinLength(3), MaxLength(15)]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        //Relations

        public RefreshToken RefreshToken { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; } = null!;
    }
}
