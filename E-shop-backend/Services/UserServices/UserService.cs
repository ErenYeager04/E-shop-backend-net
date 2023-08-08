using AutoMapper;
using Azure.Core;
using E_shop_backend.Data;
using E_shop_backend.Dtos;
using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace E_shop_backend.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public User GetUser(string email)
        {
            var user = _context.Users.FirstOrDefault(U => U.Email == email);
            return user;
        }

        public async Task<ServiceResponse<User>> LogUser(LoginDto user)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<User>();
            // Checking if user exists
            if (!UserExists(user.Email))
            {
                serviceResponse.Message = "User not found";
                serviceResponse.Success = false;
                return serviceResponse;
            }
            // Retrieving user from Db
            var DBuser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            // Verifying password
            if (!BCryptNet.Verify(user.Password, DBuser.Password))
            {
                serviceResponse.Message = "Invalid password";
                serviceResponse.Success = false;
                return serviceResponse;
            }

            serviceResponse.Data = DBuser;
            serviceResponse.Success = true;
            return serviceResponse;
        }

        public async Task<ServiceResponse<User>> SignUser(RegisterDto user)
        {
            // Initializing service response
            var serviceResponse = new ServiceResponse<User>();
            // Checking if user already exists
            if (UserExists(user.Email))
            {
                serviceResponse.Message = "User already exists";
                serviceResponse.Success = false;
                return serviceResponse;
            }
            // Hashing the password
            user.Password = HashPassword(user.Password);

            var usersMap = _mapper.Map<User>(user);

            try
            {
                await _context.Users.AddAsync(usersMap);
                await _context.SaveChangesAsync();
                serviceResponse.Data = usersMap;
                serviceResponse.Success = true;
                return serviceResponse;

            }catch(Exception ex)
            {
                serviceResponse.Message = "Error occured while saving the user";
                serviceResponse.Success = false;
                return serviceResponse;
            }
            
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
        private string HashPassword(string password)
        {
            // Generate a salt for the password hash
            string salt = BCryptNet.GenerateSalt();

            // Hash the password using BCrypt with the generated salt
            string hashedPassword = BCryptNet.HashPassword(password, salt);

            // Return the hashed password
            return hashedPassword;
        }
    }
}
