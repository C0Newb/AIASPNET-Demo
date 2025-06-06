using AIASPNET.Domain.ToDo.Models;
using AIASPNET.Domain.ToDo.Repository;
using AIASPNET.Domain.ToDo.Services;
using AIASPNET.Infrastructure.MongoDB;

namespace AIASPNET.Domain.ToDo.Config;

public static class ToDoExtensions
{
  public static IServiceCollection AddToDo(this IServiceCollection services) =>
    services
      .AddMongoRepository<IToDoTaskRepository, ToDoTaskRepository, ToDoTask>()
      .AddScoped<IToDoService, ToDoService>();
}
