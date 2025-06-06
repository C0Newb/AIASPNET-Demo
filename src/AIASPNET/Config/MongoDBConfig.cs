using System.ComponentModel.DataAnnotations;

namespace AIASPNET.Config;

public class MongoDBConfig
{
  public const string SectionName = "MongoDB";

  [Required]
  public required string ConnectionString { get; set; }

  [Required]
  public required string DatabaseName { get; set; }
}
