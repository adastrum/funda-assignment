using Assignment.Api.Features.Shared;
using Assignment.Api.Features.Statistics;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddSharedServices(builder.Configuration);
builder.Services.AddStatisticsServices(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/api/health");
app.MapStatisticsEndpoints();
app.Run();

public partial class Program
{
}