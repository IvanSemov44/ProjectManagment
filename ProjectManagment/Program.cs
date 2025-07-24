using Serilog;

using Microsoft.EntityFrameworkCore;

using Application;
using Application.Absrtactions;
using Contracts;
using Domain;
using Infrastructure;
using LoggingService;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<ICustomLogger, CustomLogger>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");


builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        connectionString,
        x => x.MigrationsAssembly("ProjectManagement")));

builder.Host.UseSerilog((hostContext, configuration) =>
{
    configuration.ReadFrom.Configuration(hostContext.Configuration);
});

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
