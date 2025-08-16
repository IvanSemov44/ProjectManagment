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
using Contracts.Projects;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<ICustomLogger, CustomLogger>();
builder.Services.AddScoped<IDataShapingService<ProjectResponse>, DataShapingService<ProjectResponse>>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateProjectRequestValidator));

builder.Services.AddMinimalEndpoints();

builder.ConfigureCors();
builder.ConfigureSwagger();
builder.ConfigureLogging();
builder.ConfigureDatabase();
builder.ConfigureApiVersioning();
builder.ConfigureOutputCaching();
builder.ConfigureRateLimiting();
builder.ConfigureMicrosoftIdentity();
builder.ConfigureAuthentication();
builder.ConfigureAuthorization();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//Configure the HTTP request pipeline.

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

public partial class Program { }

//app.UseExceptionHandler(); // 1. Exception handling should be first to catch all errors.
//app.UseHttpsRedirection(); // 2. HTTPS redirection comes early to ensure secure connections.
//app.UseCors("AllowAll"); // 3. CORS is handled early, before authentication and authorization.
//app.UseOutputCache(); // 4. Caching comes after CORS and redirection.
//app.UseAuthentication(); // 5. Authentication identifies the user.
//app.UseAuthorization(); // 6. Authorization checks if the authenticated user has access.
//app.UseRateLimiter(); // 7. Rate limiting should be applied after auth to correctly identify clients.

//// The following are not middleware in the traditional sense, but configuration for the pipeline.
//await app.AddRoles(); // This is likely a service that runs once at startup.
//app.AddVersionedEndpoints(); // Endpoint mapping must be done after authorization.
//app.AddSwagger(); // Swagger should also be mapped after the main middleware pipeline is configured.
//app.Run();