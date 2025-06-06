using AIASPNET.Domain.ToDo.Models;
using MongoDB.Driver;

namespace AIASPNET.Domain.ToDo.Repository;

public class ToDoTaskRepository : IToDoTaskRepository
{
  private readonly IMongoCollection<ToDoTask> _collection;
  public IMongoCollection<ToDoTask> Collection => _collection;

  public ToDoTaskRepository(IMongoDatabase database)
  {
    _collection = database.GetCollection<ToDoTask>("ToDoTasks");
  }

  public Task InitializeIndexes() =>
    _collection.Indexes.CreateManyAsync(
      [
        new CreateIndexModel<ToDoTask>(
          keys: Builders<ToDoTask>
            .IndexKeys.Ascending(task => task.Id)
            .Ascending(task => task.Title),
          options: new CreateIndexOptions() { Unique = true, Name = "id_name_unique" }
        ),
      ]
    );

  public Task CreateAsync(ToDoTask document) => _collection.InsertOneAsync(document);

  public Task UpdateAsync(ToDoTask document) =>
    _collection.ReplaceOneAsync(
      FilterById(document.Id),
      document,
      new ReplaceOptions { IsUpsert = true }
    );

  public Task DeleteAsync(ToDoTask document) => _collection.DeleteOneAsync(FilterById(document.Id));

  public Task<bool> ExistsAsync(ToDoTask document) =>
    _collection.Find(FilterById(document.Id)).AnyAsync();

  public Task<ToDoTask?> GetByIdAsync(Guid id) =>
    _collection.Find(FilterById(id)).FirstOrDefaultAsync()!;

  public async Task<IEnumerable<ToDoTask>> GetAllTasks() =>
    (await _collection.Find(Builders<ToDoTask>.Filter.Empty).ToCursorAsync()).ToEnumerable();

  private static FilterDefinition<ToDoTask> FilterById(Guid id) =>
    Builders<ToDoTask>.Filter.Eq(task => task.Id, id);
}
