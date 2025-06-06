using AIASPNET.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace AIASPNET.Domain.ToDo.Models;

/// <summary>
/// Represents a task to do.
/// </summary>
public record ToDoTask : IDocument
{
  [BsonRepresentation(MongoDB.Bson.BsonType.String)]
  public required Guid Id { get; init; } = Guid.NewGuid();

  public DateTime CreatedTime { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;

  /// <summary>
  /// Title of the task.
  /// </summary>
  public required string Title { get; init; }

  /// <summary>
  /// Description of the task.
  /// </summary>
  public required string Description { get; init; }
}
