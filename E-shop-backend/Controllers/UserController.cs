using AutoMapper;
using Azure.Core;
using E_shop_backend.Dtos;
using E_shop_backend.Models;
using E_shop_backend.Services.RefreshTokenService;
using E_shop_backend.Services.ReviewService;
using E_shop_backend.Services.UserServices;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_shop_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IConfiguration _configuration;
        private readonly IReviewService _reviewService;

        public UserController(
            IUserService userService,
            IRefreshTokenService refreshTokenService,
            IConfiguration configuration,
            IReviewService reviewService
            )
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _configuration = configuration;
            _reviewService = reviewService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(RegisterDto request,[FromServices] IValidator<RegisterDto> validator)
        {
            // Validating object with fluentValidator
            ValidationResult validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(errorMessage);
            }
            // Signing in user
            var result = await _userService.SignUser(request);
            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            // Getting user
            var user = result.Data;
            // Generating tokens 
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, user);
            var token = GenerateJwtToken(user.Id, user.Email);

            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request,[FromServices] IValidator<LoginDto> validator)
        {
            ValidationResult validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(errorMessage);
            }

            // Signing in user
            var result = await _userService.LogUser(request);
            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            // Getting user
            var user = result.Data;
            // Generating tokens 
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, user);
            var token = GenerateJwtToken(user.Id, user.Email);

            return Ok(token);
        }

        [HttpGet("refresh/{email}")]
        public IActionResult RefreshToken(string email)
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
        [Authorize]
        public async Task<IActionResult> CreateReview(ReviewDto review,[FromServices] IValidator<ReviewDto> validator)
        {
            ValidationResult validationResult = validator.Validate(review);

            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(errorMessage);
            }

            // Creating a review
            var result = await _reviewService.CreateReview(review);

            // Checking if everything went correct
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);

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
                Secure = true,
                Expires = newRefreshToken.Expires,
                SameSite = SameSiteMode.None,

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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

    }
}
