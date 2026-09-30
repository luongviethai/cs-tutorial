using CS_Tutorial.Data;
using CS_Tutorial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    public async Task<ActionResult<List<Todo>>> GetAll()
    {
        var todos = await _context.Todos.AsNoTracking().OrderBy(t => t.Id).ToListAsync();

        return Ok(todos);
    }

    [HttpPost]

    public async Task<ActionResult<Todo>> Create(Todo newTodo)
    {

        _context.Todos.Add(newTodo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = newTodo.Id }, newTodo);

    }

    [HttpGet("{id:int}")]

    public async Task<ActionResult<Todo>> GetById(int id)
    {
        var todo = await _context.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

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