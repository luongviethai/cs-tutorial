namespace CS_Tutorial.Models;

public class Todo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int UserId { get; set; }

    public bool? IsCompleted { get; set; }

}

