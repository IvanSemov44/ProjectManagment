using Serilog;
using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Application;
using Application.Absrtactions;

using Contracts;
using Domain;
using Infrastructure;
using LoggingService;

using ProjectManagement.Endpoints;
using ProjectManagement.Endpoints.Extensions;
using ProjectManagement.Middleware;
using ProjectManagement.Validators;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<ICustomLogger, CustomLogger>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateProjectRequestValidator));

builder.Services.AddMinimalEndpoints();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");


builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        connectionString,
        x => x.MigrationsAssembly("ProjectManagement")));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Management Api",
        Version = "v1",
        Description = "Api for managing projects and related subtasks",
        Contact = new OpenApiContact
        {
            Name = "Code Maze",
            Email = "info@code-maze.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensoure.org/licenses/MIT")
        }
    });
});
builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog((hostContext, configuration) =>
{
    configuration.ReadFrom.Configuration(hostContext.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.RegisterProjectEndpoints();
app.RegisterMinimalEndpoints();

app.Run();