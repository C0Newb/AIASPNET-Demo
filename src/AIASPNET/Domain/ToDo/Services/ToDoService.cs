using AIASPNET.Domain.ToDo.Models;
using AIASPNET.Domain.ToDo.Repository;

namespace AIASPNET.Domain.ToDo.Services;

public class ToDoService : IToDoService
{
  private readonly IToDoTaskRepository _toDoTaskRepository;

  public ToDoService(IToDoTaskRepository toDoTaskRepository)
  {
    _toDoTaskRepository = toDoTaskRepository;
  }

  public async Task<IEnumerable<ToDoTask>> GetAllTasksAsync()
  {
    return await _toDoTaskRepository.GetAllTasks();
  }

  public async Task<ToDoTask?> GetTaskByIdAsync(Guid id) =>
    await _toDoTaskRepository.GetByIdAsync(id)
    ?? throw new KeyNotFoundException($"No such task {id}");

  public async Task CreateTask(ToDoTask task)
  {
    if (await _toDoTaskRepository.ExistsAsync(task))
    {
      await _toDoTaskRepository.UpdateAsync(task);
    }
    else
    {
      await _toDoTaskRepository.CreateAsync(task);
    }
  }

  public async Task CreateTaskAsync(ToDoTask task)
  {
    ArgumentNullException.ThrowIfNull(task);
    await _toDoTaskRepository.CreateAsync(task);
  }

  public async Task UpdateTaskAsync(ToDoTask task)
  {
    ArgumentNullException.ThrowIfNull(task);
    await _toDoTaskRepository.UpdateAsync(task);
  }

  public async Task DeleteTaskAsync(Guid id)
  {
    var task =
      await _toDoTaskRepository.GetByIdAsync(id)
      ?? throw new KeyNotFoundException($"Task with ID {id} not found.");
    await _toDoTaskRepository.DeleteAsync(task);
  }
}
