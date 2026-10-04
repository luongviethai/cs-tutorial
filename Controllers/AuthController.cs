using CS_Tutorial.Data;
using CS_Tutorial.Dtos;
using CS_Tutorial.Models;
using CS_Tutorial.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CS_Tutorial.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, IPasswordHasher<User> passwordHasher, TokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Chuẩn hóa email: unique index của PostgreSQL phân biệt hoa/thường
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(u => u.Email == email))
            return Problem(detail: "Email đã được sử dụng.", statusCode: StatusCodes.Status409Conflict);

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
        };
        // Chỉ lưu chuỗi băm, không bao giờ lưu mật khẩu gốc
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(ToAuthResponse(user));
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

        // Không tìm thấy email và sai mật khẩu trả về CÙNG một thông báo
        if (user is null ||
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            return Problem(detail: "Email hoặc mật khẩu không đúng.", statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(ToAuthResponse(user));
    }

    private AuthResponse ToAuthResponse(User user) => new()
    {
        Token = _tokenService.CreateToken(user),
        Name = user.Name,
        Email = user.Email,
        Role = user.Role,
    };
}
