using System.Text;
using CS_Tutorial.Data;
using Microsoft.EntityFrameworkCore;
using CS_Tutorial.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CS_Tutorial.Settings;
using CS_Tutorial.Models;
using CS_Tutorial.Services;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtSection = builder.Configuration.GetSection("Jwt");

var jwt = jwtSection.Get<JwtSettings>()
    ?? throw new InvalidOperationException("Thiếu mục cấu hình 'Jwt'.");

if (jwt.SecretKey.Length < 32)
    throw new InvalidOperationException("Jwt:SecretKey phải dài ít nhất 32 ký tự (đặt bằng user-secrets).");

builder.Services.Configure<JwtSettings>(jwtSection);   // Để bước 3 inject IOptions<JwtSettings>

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,             // Từ chối token đã hết hạn
            ValidateIssuerSigningKey = true,     // Kiểm tra chữ ký: chống sửa token (mục 3)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddSingleton<TokenService>();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
