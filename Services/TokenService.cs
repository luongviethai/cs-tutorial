using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CS_Tutorial.Models;
using CS_Tutorial.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CS_Tutorial.Services;

public class TokenService
{
    private readonly JwtSettings _jwt;

    // IOptions<JwtSettings> có sẵn trong DI nhờ dòng Configure<JwtSettings> ở Program.cs
    public TokenService(IOptions<JwtSettings> options)
    {
        _jwt = options.Value;
    }

    public string CreateToken(User user)
    {
        // Claims: những thông tin về người dùng được ghi vào token
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role),
        };

        // Chữ ký: tạo từ secret key, dùng để chống sửa token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
