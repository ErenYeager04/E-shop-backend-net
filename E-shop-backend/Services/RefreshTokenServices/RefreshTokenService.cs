using E_shop_backend.Data;
using E_shop_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace E_shop_backend.Services.RefreshTokenService
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly DataContext _context;

        public RefreshTokenService(DataContext context)
        {
            _context = context;
        }

        public RefreshToken CreateToken(RefreshToken newToken)
        {
            _context.RefreshTokens.Add(newToken);
            _context.SaveChanges();
            return newToken;
        }

        public RefreshToken GetToken(int userId)
        {
            var token = _context.RefreshTokens.FirstOrDefault(r => r.UserId == userId);
            if (token == null)
            {
                throw new Exception("Product doesnt exist");
            }
            return token;
        }

        public RefreshToken UpdateToken(RefreshToken newToken)
        {
            // Get users already existing token
            var existingToken = _context.RefreshTokens.FirstOrDefault(rt => rt.UserId == newToken.UserId);
            if (existingToken == null)
            {
                throw new Exception("Token doesnt exist");
            }
            // Assign new data
            existingToken.Token = newToken.Token;
            existingToken.Expires = newToken.Expires;

            _context.RefreshTokens.Update(existingToken);
            _context.SaveChanges();
            return existingToken;
        }

        public bool UserHaveToken(int userId)
        {
            return _context.RefreshTokens.Any(r => r.UserId == userId);
        }
    }
}
