using AIASPNET.Config;

namespace AIASPNET.Infrastructure.MongoDB;

/// <summary>
/// Ensures that MongoDB indexes are initialized on application startup.
/// </summary>
public class MongoIndexInitializer : IHostedService
{
  private readonly MongoDBConfig _config;
  private readonly IIndexInitializer[] _indexers;

  public MongoIndexInitializer(MongoDBConfig mongoDBConfig, IEnumerable<IIndexInitializer> indexers)
  {
    ArgumentNullException.ThrowIfNull(mongoDBConfig);
    ArgumentNullException.ThrowIfNull(indexers);
    _indexers = [.. indexers];

    if (_indexers.Length == 0)
    {
      throw new InvalidOperationException(
        "No indexers found. Ensure that you have registered at least one IIndexInitializer in the DI container."
      );
    }

    _config = mongoDBConfig;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(_config.ConnectionString))
    {
      return;
    }

    await Task.WhenAll(_indexers.Select(indexer => indexer.InitializeIndexes()));
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
