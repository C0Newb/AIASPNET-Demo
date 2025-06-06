using MongoDB.Driver;

namespace AIASPNET.Infrastructure.MongoDB;

public interface IMongoRepository<TDocument> : IIndexInitializer
  where TDocument : IDocument
{
  IMongoCollection<TDocument> Collection { get; }

  /// <summary>
  /// Checks whether a document exists in the MongoDB collection.
  /// </summary>
  /// <param name="document">Document to check for.</param>
  /// <returns>If the document exists within the MongoDB collection.</returns>
  Task<bool> ExistsAsync(TDocument document);

  /// <summary>
  /// Inserts a new document into the MongoDB collection.
  /// </summary>
  /// <param name="document">Document to add to the collection.</param>
  /// <returns>Result of the insert operation.</returns>
  Task CreateAsync(TDocument document);

  /// <summary>
  /// Deletes a document from the MongoDB collection.
  /// </summary>
  /// <param name="document">Document to delete.</param>
  /// <returns>Results of the delete operation.</returns>
  Task DeleteAsync(TDocument document);
}
