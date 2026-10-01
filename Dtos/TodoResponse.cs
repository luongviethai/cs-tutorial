using System.ComponentModel.DataAnnotations;
namespace CS_Tutorial.Dtos;


public record TodoResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool? IsCompleted { get; set; }
}

public record CreateTodoRequest
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    public string Name { get; set; } = string.Empty;
    public bool? IsCompleted { get; set; }
}