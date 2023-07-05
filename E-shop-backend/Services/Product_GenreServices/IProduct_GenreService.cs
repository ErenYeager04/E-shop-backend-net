using E_shop_backend.Models;

namespace E_shop_backend.Services.Product_GenreService
{
    public interface IProduct_GenreService
    {
        ICollection<Product_Genre> GetProduct_Genre(int ProductId);
        Product_Genre CreateProduct_Genre(Product_Genre product_Genre);
        Product_Genre UpdateProduct_Genre(Product_Genre product_Genre);
        Product_Genre DeleteProduct_Genre(Product_Genre product_Genre);
        bool Product_GenreExists(Product_Genre product_Genre);
    }
}
