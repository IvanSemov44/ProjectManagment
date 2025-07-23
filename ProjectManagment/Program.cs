using Contracts;
using LoggingService;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((hostContext, configuration) =>
{
    configuration.ReadFrom.Configuration(hostContext.Configuration);
});

builder.Services.AddSingleton<ICustomLogger, CustomLogger>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/test", (ICustomLogger logger) =>
{
    logger.LogDebug("This is a debug message");
    logger.LogInformation("This is a information message");
    logger.LogWarning("This is a warning message");
    logger.LogError("This is error message");
});

app.Run();
