using AIASPNET.Domain.ToDo.Models;
using AIASPNET.Infrastructure.MongoDB;

namespace AIASPNET.Domain.ToDo.Repository;

public interface IToDoTaskRepository : IMongoRepository<ToDoTask>
{
  Task<IEnumerable<ToDoTask>> GetAllTasks();
  Task<ToDoTask?> GetByIdAsync(Guid id);
  Task UpdateAsync(ToDoTask document);
}
