using CS_Tutorial.Models;

namespace CS_Tutorial.Services;

public interface ITodoService
{
    List<Todo> GetAll();
    Todo? GetById(int id);
    Todo Create(Todo newTodo);
    Todo? Update(int id, Todo updatedTodo);
    bool Delete(int id);
}