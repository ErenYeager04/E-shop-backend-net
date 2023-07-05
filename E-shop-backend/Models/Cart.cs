namespace E_shop_backend.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Cart_Product> CartProducts { get; set; } = null!;
    }
}
