using AIASPNET.Domain.AI.Services;
using AIASPNET.Domain.ToDo.Config;
using AIASPNET.Infrastructure.HTTP;
using AIASPNET.Infrastructure.MongoDB;

var _ = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var builder = WebApplication.CreateBuilder(args);

// Data Access
builder.Services.AddMongoDB();

// Domains
builder.Services.AddToDo();

// REST
AIService.SetMvcBuilder(builder.Services.AddControllers());

// Open API
builder.Services.ConfigureOpenApi();

var app = builder.Build();
AIService.SetWebApplication(app);

app.UseHttpsRedirection();

// Swagger UI
app.UseOpenApi();

// Controllers
app.MapControllers();

while (!AIService.Created)
{
  // Wait for ChatGPT to create the controller
  Thread.Sleep(250);
}

await app.RunAsync();
