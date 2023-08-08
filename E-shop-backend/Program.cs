using BCrypt.Net;
using E_shop_backend.Data;
using E_shop_backend.Dtos;
using E_shop_backend.Migrations;
using E_shop_backend.Models;
using E_shop_backend.Services.CartServices;
using E_shop_backend.Services.ProductServices;
using E_shop_backend.Services.RefreshTokenService;
using E_shop_backend.Services.ReviewService;
using E_shop_backend.Services.UserServices;
using E_shop_backend.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<DataContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Adding swagger gen a auth function to test auth
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using Bearer scheme",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
// Registering validators
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
builder.Services.AddScoped<IValidator<ReviewDto>, ReviewDtoValidator>();
builder.Services.AddScoped<IValidator<ReqProductDto>, ReqProductDtoValidator>();
builder.Services.AddScoped<IValidator<Cart_ProductDto>, Cart_ProductDtoValidator>();

// Registering services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Auth register
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"))),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true, // Enable lifetime validation of the token
            ClockSkew = TimeSpan.Zero, // Set clock skew to zero to enforce strict token expiration
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "text/plain";

                // Check if the token is expired
                if (context.AuthenticateFailure?.GetType() == typeof(SecurityTokenExpiredException))
                {
                    var errorMessage = "Token expired. Please log in again.";
                    return context.Response.WriteAsync(errorMessage);
                }

                // Check if the user is not in the "Admin" role
                var user = context.HttpContext.User;
                if (!user.IsInRole("Admin"))
                {
                    var errorMessage = "You don't have access.";
                    return context.Response.WriteAsync(errorMessage);
                }

                // If the user is in the "Admin" role, send the default error message
                var defaultErrorMessage = "You are not authorized.";
                return context.Response.WriteAsync(defaultErrorMessage);
            }
        };
    });
//Add cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("corspolicy", builder =>
    {
        builder.WithOrigins(Environment.GetEnvironmentVariable("FRONTEND"))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
// Registering cors
app.UseCors("corspolicy");

app.UseHttpsRedirection();
// Adding authentication
app.UseAuthentication();

app.UseAuthorization();
// Unexpected exception handler
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
