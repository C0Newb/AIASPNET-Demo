namespace AIASPNET.Domain.AI.Config;

public class AIConfig
{
  public static readonly string SectionName = "OpenAI";

  public required string ApiKey { get; set; }
  public required string ChatGptModel { get; set; } = "gpt-3.5-turbo";
  public required string ProjectId { get; set; }
}
