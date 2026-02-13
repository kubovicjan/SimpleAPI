using SimpleAPI.Core;
using SimpleAPI.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

var remoteServiceConfig = builder.Configuration
    .GetSection(nameof(ApiConfiguration))
    .Get<ApiConfiguration>();

if (remoteServiceConfig is null)
{
    Console.WriteLine("Remote service configuration is missing. Please provide the necessary configuration in appsettings.json or environment variables.");
    return 1;
}

// Add services to the container.

builder.Services.AddControllers();
builder.AddCoreServices(remoteServiceConfig);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

return 0;