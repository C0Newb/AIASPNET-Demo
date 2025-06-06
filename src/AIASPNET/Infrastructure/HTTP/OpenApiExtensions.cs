using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace AIASPNET.Infrastructure.HTTP;

public static class OpenApiExtensions
{
  public static void ConfigureOpenApi(this IServiceCollection services)
  {
    services.AddEndpointsApiExplorer();
    services.AddOpenApi(options =>
    {
      options.AddDocumentTransformer(
        (document, context, cancellationToken) =>
        {
          document.Info.Title = "AI Demo API";
          document.Info.Description = "Self generating API";
          document.Info.Version =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "X1.0.0";
          document.Servers.Clear();
          document.Servers.Add(
            new OpenApiServer { Url = "http://localhost:8080", Description = "Localhost" }
          );
          return Task.CompletedTask;
        }
      );
      options.AddSchemaTransformer(
        (schema, context, cancellationToken) =>
        {
          var type = context.JsonTypeInfo.Type;
          if (!typeof(Enum).IsAssignableFrom(type))
            return Task.CompletedTask;

          schema.Type = "string";
          schema.Enum.Clear();
          foreach (var enumName in Enum.GetNames(type))
          {
            var memberInfo = type.GetMember(enumName)
              .FirstOrDefault(info => info.DeclaringType == type);
            var enumMemberAttribute = memberInfo
              ?.GetCustomAttributes(typeof(EnumMemberAttribute), false)
              .OfType<EnumMemberAttribute>()
              .FirstOrDefault();
            var label =
              enumMemberAttribute == null || string.IsNullOrWhiteSpace(enumMemberAttribute.Value)
                ? enumName
                : enumMemberAttribute.Value;
            schema.Enum.Add(new OpenApiString(label));
          }

          return Task.CompletedTask;
        }
      );
    });
  }

  public static void UseOpenApi(this WebApplication app)
  {
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
      options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Demo API V1");
    });
  }
}
