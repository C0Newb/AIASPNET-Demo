namespace AIASPNET.Infrastructure.MongoDB;

public interface IDocument
{
  DateTime CreatedTime { get; init; }

  DateTime UpdatedTime { get; set; }
}
