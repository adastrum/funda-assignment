var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/api/health");
app.Run();

public partial class Program
{
}