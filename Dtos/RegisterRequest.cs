using System.ComponentModel.DataAnnotations;

namespace CS_Tutorial.Dtos;

public record RegisterRequest
{
    [Required(ErrorMessage = "Tên không được để trống")]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
    public string Password { get; init; } = string.Empty;
}
