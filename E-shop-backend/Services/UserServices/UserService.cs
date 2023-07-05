using E_shop_backend.Data;
using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace E_shop_backend.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public User CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User DeleteUser(int Id)
        {
            var user = _context.Users.Find(Id);

            if (user == null)
            {
                throw new Exception("User doesnt exist");
            }

            _context.Users.Remove(user); 
            _context.SaveChanges(); 

            return user;

        }

        public User GetUser(string email)
        {
            var user = _context.Users.FirstOrDefault(U => U.Email == email);
            return user;
        }
            
        public ICollection<User> GetUsers()
        {
            var response = _context.Users.OrderBy(U => U.Id).ToList();
            return response;
        }

        public User UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return user;
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
