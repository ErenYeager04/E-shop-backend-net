using E_shop_backend.Models;

namespace E_shop_backend.Services.UserServices
{
    public interface IUserService
    {
        ICollection<User> GetUsers();
        User GetUser(string email);
        User CreateUser(User user);
        User UpdateUser(User user);
        User DeleteUser(int Id);
        bool UserExists(string email);

    }
}
