using System.ComponentModel.DataAnnotations;

namespace CS_Tutorial.Dtos;

public record LoginRequest
{
    [Required(ErrorMessage = "Email không được để trống")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; init; } = string.Empty;
}
