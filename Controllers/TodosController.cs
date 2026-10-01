using CS_Tutorial.Data;
using CS_Tutorial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS_Tutorial.Dtos;

namespace CS_Tutorial.Controllers;

[ApiController]
[Route("api/[controller]")]


public class TodosController : ControllerBase
{

    private readonly AppDbContext _context;

    public TodosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]

    public async Task<ActionResult<List<TodoResponse>>> GetAll()
    {
        var todos = await _context.Todos.AsNoTracking().OrderBy(t => t.Id).Select(t => new TodoResponse
        {
            Id = t.Id,
            Name = t.Name,
            IsCompleted = t.IsCompleted,
        }).ToListAsync();

        return Ok(todos);
    }

    [HttpPost]

    public async Task<ActionResult<TodoResponse>> Create(CreateTodoRequest request)
    {
        var newTodo = new Todo
        {
            Name = request.Name,
            IsCompleted = request.IsCompleted
        };

        _context.Todos.Add(newTodo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = newTodo.Id }, new TodoResponse
        {
            Id = newTodo.Id,
            Name = newTodo.Name,
            IsCompleted = newTodo.IsCompleted
        });

    }

    [HttpGet("{id:int}")]

    public async Task<ActionResult<TodoResponse>> GetById(int id)
    {
        var todo = await _context.Todos.AsNoTracking().Select(t => new TodoResponse
        {
            Id = t.Id,
            Name = t.Name,
            IsCompleted = t.IsCompleted
        }).FirstOrDefaultAsync(t => t.Id == id);

        if (todo is null)
            return NotFound($"Không tìm thấy Todo có Id = {id}.");

        return Ok(todo);
    }

    [HttpDelete("{id:int}")]

    public async Task<ActionResult> Delete(int id)
    {
        var todo = await _context.Todos.FindAsync(id);

        if (todo is null)
            return NotFound($"Không tìm thấy Todo có Id = {id}.");

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Todo>> Update(int id, Todo updatedTodo)
    {
        if (id != updatedTodo.Id)
            return BadRequest("Id không hợp lệ.");

        var todo = await _context.Todos.FindAsync(id);

        if (todo is null)
            return NotFound($"Không tìm thấy Todo có Id = {id}.");

        todo.Name = updatedTodo.Name;
        todo.IsCompleted = updatedTodo.IsCompleted;

        await _context.SaveChangesAsync();

        return Ok(todo);
    }

}