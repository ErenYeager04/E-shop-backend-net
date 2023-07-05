using System.ComponentModel.DataAnnotations;

namespace E_shop_backend.Dtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        [Required, MinLength(3), MaxLength(15)]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
