using FluentValidation;
using Application;
using Application.Absrtactions;
using Contracts;
using Domain;
using Infrastructure;
using LoggingService;
using ProjectManagement.Endpoints.Extensions;
using ProjectManagement.Middleware;
using ProjectManagement.Extensions;
using ProjectManagement.Validators.Projects;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<ICustomLogger, CustomLogger>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateProjectRequestValidator));

builder.Services.AddMinimalEndpoints();

builder.Services.AddAuthorization();

builder.ConfigureCors();
builder.ConfigureSwagger();
builder.ConfigureLogging();
builder.ConfigureDatabase();
builder.ConfigureApiVersioning();
builder.ConfigureOutputCaching();
builder.ConfigureRateLimiting();
builder.ConfigureMicrosoftIdentity();
builder.ConfigureAuthentication();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

await app.AddRoles();

app.UseRateLimiter();

app.UseCors("AllowAll");

app.UseOutputCache();

app.AddVersionedEndpoints();

app.AddSwagger();

app.Run();