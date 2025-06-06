using AIASPNET.Domain.ToDo.Models;

namespace AIASPNET.Domain.ToDo.Services;
public interface IToDoService
{
  Task CreateTask(ToDoTask task);
  Task CreateTaskAsync(ToDoTask task);
  Task DeleteTaskAsync(Guid id);
  Task<IEnumerable<ToDoTask>> GetAllTasksAsync();
  Task<ToDoTask?> GetTaskByIdAsync(Guid id);
  Task UpdateTaskAsync(ToDoTask task);
}