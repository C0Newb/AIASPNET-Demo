using AIASPNET.Config;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AIASPNET.Infrastructure.MongoDB;

public static class MongoDBExtensions
{
  public static IServiceCollection AddMongoDB(this IServiceCollection services)
  {
    services
      .AddSingleton(p => p.GetRequiredService<IOptions<MongoDBConfig>>().Value)
      .AddSingleton<IMongoClient>(sp =>
      {
        var connectionString = sp.GetRequiredService<MongoDBConfig>().ConnectionString;
        var clientSettings = MongoClientSettings.FromConnectionString(connectionString);

        return new MongoClient(clientSettings);
      })
      .AddSingleton(p =>
      {
        var config = p.GetRequiredService<MongoDBConfig>();
        var client = p.GetRequiredService<IMongoClient>();
        return client.GetDatabase(config.DatabaseName);
      })
      .AddOptions<MongoDBConfig>()
      .BindConfiguration(MongoDBConfig.SectionName)
      .ValidateDataAnnotations()
      .ValidateOnStart();
    return services;
  }

  public static IServiceCollection AddMongoRepository<TService, TImplementation, TEntity>(
    this IServiceCollection services
  )
    where TImplementation : class, TService
    where TService : class, IMongoRepository<TEntity>
    where TEntity : class, IDocument
  {
    services
      .AddSingleton<TService, TImplementation>()
      .AddSingleton<IMongoRepository<TEntity>>(p => p.GetRequiredService<TService>())
      .AddSingleton<IIndexInitializer>(p => p.GetRequiredService<TService>())
      .AddHostedService<MongoIndexInitializer>();
    return services;
  }
}
