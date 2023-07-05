using AutoMapper;
using E_shop_backend.Data;
using E_shop_backend.Dtos;
using E_shop_backend.Models;
using E_shop_backend.Services.RefreshTokenService;
using E_shop_backend.Services.ReviewService;
using E_shop_backend.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace E_shop_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IConfiguration _configuration;
        private readonly IReviewService _reviewService;

        public UserController(IUserService userService,
            IMapper mapper, 
            IRefreshTokenService refreshTokenService,
            IConfiguration configuration,
            IReviewService reviewService
            )
        {
            _userService = userService;
            _mapper = mapper;
            _refreshTokenService = refreshTokenService;
            _configuration = configuration;
            _reviewService = reviewService;
        }

        [HttpPost("signin")]
        public ActionResult<string> SignIn(RegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_userService.UserExists(request.Email))
            {
                return BadRequest("User already exists");
            }
            var passwordHash = HashPassword(request.Password);
            request.Password = passwordHash;

            var usersMap = _mapper.Map<User>(request);
            try
            {
                var user = _userService.CreateUser(usersMap);

                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(refreshToken, user);
                return GenerateJwtToken(user.Id, user.Email);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("login")]
        public ActionResult<string> Login(LoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userService.GetUser(request.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!VerifyPassword(request.Password, user.Password))
            {
                return BadRequest("Invalid password");
            }
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, user);

            return GenerateJwtToken(user.Id, user.Email);

        }

        [HttpPost("refresh")]
        public ActionResult<string> RefreshToken(string email)
        {
            // Geting token from httponly
            var refreshToken = Request.Cookies["refreshToken"];

            var user = _userService.GetUser(email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            // Comparing token from user to token from database
            var RefreshToken = _refreshTokenService.GetToken(user.Id);
            if (!RefreshToken.Token.Equals(refreshToken))
            {
                return Unauthorized("Invalid refresh token");
            }
            // Cheking if it expired
            else if (RefreshToken.Expires < DateTime.Now)
            {
                return Unauthorized("Token expired");
            }

            string token = GenerateJwtToken(user.Id, user.Email);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken, user);

            return Ok(token);
        }

        [HttpPost("createReview")]
        public IActionResult CreateReview(ReviewDto review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reviewMap = _mapper.Map<Review>(review);

            try
            {
                var newReview = _reviewService.CreateReview(reviewMap);
                return Ok(newReview);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("getUserReviews/{userId}")]
        public IActionResult GetUserReviews(int userId)
        {
            try
            {
                var userReviews = _reviewService.GetUserReviews(userId);
                return Ok(userReviews);
            } catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                // Token itself
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                // Expire date
                Expires = DateTime.Now.AddDays(30),
                Created = DateTime.Now
            };
            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,

            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
            // If user alredy have token update it, if not create
            newRefreshToken.UserId = user.Id;
            if (_refreshTokenService.UserHaveToken(user.Id))
            {
                _refreshTokenService.UpdateToken(newRefreshToken);
            }
            _refreshTokenService.CreateToken(newRefreshToken);


        }

        private string GenerateJwtToken(int userId, string Email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings")["Token"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                 expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
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

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCryptNet.Verify(password, hashedPassword);
        }
    }
}
