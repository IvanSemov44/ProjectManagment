using Serilog;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Application;
using Application.Absrtactions;
using Contracts;
using Domain;
using Infrastructure;
using LoggingService;
using ProjectManagement.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<ICustomLogger, CustomLogger>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAutoMapper(typeof(Program));

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
app.UseExceptionHandler();

app.MapGet("api/projects", async ([
    FromServices] IServiceManager serviceManager,
    CancellationToken cancellationToken) =>
{
    var projects = await serviceManager.ProjectService
    .GetAllProjectsAsync(cancellationToken);
    return Results.Ok(projects);
});

app.MapGet("api/projects/{id:guid}", async (
    [FromRoute] Guid id,
    [FromServices] IServiceManager serviceManager,
    CancellationToken cancellationToken) =>
{
    var project = await serviceManager.ProjectService
    .GetProjectAsync(
        id,
        trackChanges: false,
        cancellationToken);

    return Results.Ok(project);
});

app.MapGet("api/projects/{projectId:guid}/subtasks", async (
    [FromRoute] Guid projectId,
    [FromServices] IServiceManager serviceManager,
    CancellationToken cancellationToken) =>
{
    var subtastks = await serviceManager.SubtaskService
    .GetAllSubtasksForProjectAsync(projectId, cancellationToken);

    return Results.Ok(subtastks);
});

app.MapGet("api/projects/{projectId:guid}/subtasks/{id:guid}", async (
    [FromRoute] Guid projectId,
    [FromRoute] Guid id,
    [FromServices] IServiceManager serviceManager,
    CancellationToken cancellationToken) =>
{
    var subtask = await serviceManager.SubtaskService
    .GetSubtaskForProjectAsync(
        projectId,
        id,
        trackChanges: false,
        cancellationToken);

    return Results.Ok(subtask);
});

app.Run();