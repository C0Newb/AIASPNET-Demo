using System.ClientModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AIASPNET.Domain.AI.Config;
using AIASPNET.Domain.ToDo.Models;
using AIASPNET.Domain.ToDo.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OpenAI;
using OpenAI.Chat;

namespace AIASPNET.Domain.AI.Services;

public static class AIService
{
  private static ChatClient? _chatClient;
  private static IMvcBuilder? _mvcBuilder;
  private static WebApplication? _app;
  public static bool Created { get; private set; } = false;

  public static void SetMvcBuilder(IMvcBuilder mvcBuilder) => _mvcBuilder = mvcBuilder;

  public static void SetWebApplication(WebApplication webApp)
  {
    _app = webApp;
    if (_app != null)
    {
      Initialize();
    }
  }

  private static void Initialize()
  {
    var appSettings = File.ReadAllText(
      $"appsettings{(_app!.Environment.IsDevelopment() ? ".Development" : "")}.json"
    );

    var json =
      JsonSerializer.Deserialize<JsonObject>(appSettings)
      ?? throw new InvalidDataException("Failed to deserialize appsettings.json!");

    var config =
      JsonSerializer.Deserialize<AIConfig>(
        json["OpenAI"],
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
      ) ?? throw new InvalidOperationException("AI configuration is missing or invalid.");

    var apiKeyCredential = new ApiKeyCredential(config.ApiKey);
    var options = new OpenAIClientOptions() { ProjectId = config.ProjectId };

    _chatClient = new ChatClient(config.ChatGptModel, apiKeyCredential, options);

    AddControllerToRuntime(GenerateController());
    Created = true;
  }

  public static string GenerateController()
  {
    var code = GenerateCodeForClass(typeof(ToDoService), typeof(ToDoTask));
    Debug.WriteLine("GPT Generated code: ");
    Debug.WriteLine(code);
    return code;
  }

  private static string GenerateCodeForClass(Type service, Type model)
  {
    if (_chatClient == null)
      return string.Empty;

    var options = new ChatCompletionOptions { MaxOutputTokenCount = 1000, Temperature = 0.7f };
    var systemPrompt = GetSystemPrompt(service, model);
    Debug.WriteLine("System Prompt: " + Environment.NewLine + systemPrompt);

    ChatCompletion completion = _chatClient.CompleteChat([systemPrompt], options);
    return completion.Content[0].Text;
  }

  private static string GetSystemPrompt(Type className, Type model)
  {
    var prompt = new StringBuilder(
      $"""
      You are an expert C# developer. 
      Generate a REST controller for the model {model.FullName} that utilizes the {className.FullName} (well, the interface of that service) service to interact with the database.
      Set the class route to "api/v1/todo"
      The controller should be in the ASP.NET Core style.
      Only return the C# code, do not include any commentary, comments, or stylization syntax such as `.

      The service contains the following methods:
      """
    );

    var methods = className.GetMethods();
    foreach (var method in methods)
    {
      if (method.IsPublic && !method.IsStatic)
      {
        prompt.AppendLine(
          $"public async {method.ReturnType} {method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})"
        );
      }
    }

    prompt.AppendLine(
      $"""

      While the model {model.FullName} has the following properties:
      """
    );

    var properties = model.GetProperties();
    foreach (var property in properties)
    {
      prompt.AppendLine($"{property.Name} ({property.PropertyType.Name})");
    }

    return prompt.ToString();
  }

  public static bool AddControllerToRuntime(string code)
  {
    ArgumentNullException.ThrowIfNull(_mvcBuilder);

    if (string.IsNullOrEmpty(code))
      return false;

    var syntaxTree = CSharpSyntaxTree.ParseText(code);

    var refs = AppDomain
      .CurrentDomain.GetAssemblies()
      .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
      .Select(a => MetadataReference.CreateFromFile(a.Location));

    var compilation = CSharpCompilation.Create(
      "DynamicControllerAssembly",
      [syntaxTree],
      refs,
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
    );

    using var ms = new MemoryStream();
    var result = compilation.Emit(ms);

    if (!result.Success)
      return false;

    ms.Seek(0, SeekOrigin.Begin);
    var assembly = Assembly.Load(ms.ToArray());

    var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
    foreach (var part in partFactory.GetApplicationParts(assembly))
    {
      _mvcBuilder.PartManager.ApplicationParts.Add(part);
    }

    _mvcBuilder.PartManager.FeatureProviders.Add(new ControllerFeatureProvider());
    return true;
  }
}
