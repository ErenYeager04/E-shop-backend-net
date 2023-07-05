﻿namespace E_shop_backend.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; }

    }
}
