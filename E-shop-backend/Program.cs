using E_shop_backend.Data;
using E_shop_backend.Services.Cart_ProductServices;
using E_shop_backend.Services.CartServices;
using E_shop_backend.Services.Product_GenreService;
using E_shop_backend.Services.ProductServices;
using E_shop_backend.Services.RefreshTokenService;
using E_shop_backend.Services.ReviewService;
using E_shop_backend.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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

// Registering services
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICart_ProductService, Cart_ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProduct_GenreService, Product_GenreService>();
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
            .GetBytes(builder.Configuration.GetSection("AppSettings")["Token"])),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
