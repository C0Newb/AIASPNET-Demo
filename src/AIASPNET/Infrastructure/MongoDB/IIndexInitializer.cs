namespace AIASPNET.Infrastructure.MongoDB;

public interface IIndexInitializer
{
  /// <summary>
  /// Ensures that the necessary indexes are created in the MongoDB database.
  /// </summary>
  /// <returns></returns>
  Task InitializeIndexes();
}
