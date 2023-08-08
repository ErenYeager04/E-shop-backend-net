using E_shop_backend.Dtos;
using E_shop_backend.Models;

namespace E_shop_backend.Services.UserServices
{
    public interface IUserService
    {
        User GetUser(string email);
        bool UserExists(string email);
        Task<ServiceResponse<User>> LogUser(LoginDto user);
        Task<ServiceResponse<User>> SignUser(RegisterDto user);

    }
}
