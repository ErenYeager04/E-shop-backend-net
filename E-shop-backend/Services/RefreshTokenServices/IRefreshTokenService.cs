using E_shop_backend.Models;

namespace E_shop_backend.Services.RefreshTokenService
{
    public interface IRefreshTokenService
    {
        RefreshToken GetToken(int userId);
        RefreshToken CreateToken(RefreshToken newToken);
        RefreshToken UpdateToken(RefreshToken newToken);
        bool UserHaveToken(int userId);
    }
}
