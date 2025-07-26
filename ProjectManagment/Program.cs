using Serilog;

using Microsoft.EntityFrameworkCore;

using Application;
using Application.Absrtactions;
using Contracts;
using Domain;
using Infrastructure;
using LoggingService;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using System.Net;



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

app.MapGet("api/projects", async ([
    FromServices] IServiceManager serviceManager,
    CancellationToken cancellationToken) =>
{
    try
    {
        var projects = await serviceManager.ProjectService
        .GetAllProjectsAsync(cancellationToken);
        return Results.Ok(projects);
    }
    catch
    {
        return Results.StatusCode(
            (int)HttpStatusCode.InternalServerError);
    }
});

app.MapGet("api/projects/{id:guid}", async (
    [FromRoute] Guid id,
    [FromServices] IServiceManager serviceManager,
    CancellationToken cancellationToken) =>
{
    try
    {
        var project = await serviceManager.ProjectService
        .GetProjectAsync(
            id,
            trackChanges: false,
            cancellationToken);

        return Results.Ok(project);
    }
    catch
    {
        return Results.StatusCode(
            (int)HttpStatusCode.InternalServerError);
    }
});

app.Run();