using Application.Absrtactions;
using Contracts.Projects;
using Contracts.Requests;
using Domain;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Newtonsoft.Json;
using ProjectManagement.Middleware;
using System.Text.Json;

namespace ProjectManagement.Endpoints
{
    public static class ProjectEndpoints
    {
        public static void RegisterProjectEndpoints(
            this IEndpointRouteBuilder routeBuilder)
        {
            var group = routeBuilder.MapGroup("projects")
                .WithTags("Projects")
                .RequireRateLimiting("fixed-window");

            group.MapGet("", async (
                [AsParameters] ProjectRequestParameters requestParams,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken
                ) =>
            {
                var projects = await serviceManager.ProjectService
                .GetPagedProjectsAsync(requestParams, cancellationToken);

                return Results.Ok(projects);
            })
            .CacheOutput("FiveMinutes")
            .Produces<PagedList<ProjectResponse>>()
            .WithName(ProjectConstants.GetAllProjects);

            group.MapGet("{id:guid}", async (
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
            })
            .CacheOutput()
            .Produces<ProjectResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName(ProjectConstants.GetProjectById)
            .MapToApiVersion(1);

            group.MapGet("{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                var project = await serviceManager.ProjectService
                        .GetProjectAsync(
                            id,
                            trackChanges: false,
                            cancellationToken);
                project.Links.RemoveAll(x => x.Method is "GET");
                return Results.Ok(project);
            })
            .CacheOutput()
            .Produces<ProjectResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName(ProjectConstants.GetProjectById + " v2")
            .MapToApiVersion(2);

            group.MapPost("", async (
                [FromBody] CreateProjectRequest request,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                var project = await serviceManager.ProjectService
                .CreateProject(request, cancellationToken);

                return Results.CreatedAtRoute(
                    routeName: "GetProjectById",
                    routeValues: new { id = project.Id },
                    value: project);
            })
            .AddEndpointFilter<ValidationFilter<CreateProjectRequest>>()
            .AddEndpointFilter<CacheInvalidationFilter>()
            .Produces<ProjectResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithName(ProjectConstants.CreateProject);

            group.MapPut("{id:guid}", async (
                [FromRoute] Guid id,
                [FromBody] UpdateProjectRequest request,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                await serviceManager.ProjectService
                .UpdateProjectAsync(id, request, cancellationToken);

                return Results.NoContent();
            })
            .AddEndpointFilter<ValidationFilter<UpdateProjectRequest>>()
            .AddEndpointFilter<CacheInvalidationFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithName(ProjectConstants.UpdateProject);

            group.MapDelete("{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                await serviceManager.ProjectService
                .DeleteProjectAsync(id, cancellationToken);

                return Results.NoContent();
            })
            .AddEndpointFilter<CacheInvalidationFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(ProjectConstants.DeleteProject);

            group.MapPatch("{id:guid}", async (
                [FromRoute] Guid id,
                [FromBody] JsonElement jsonElement,
                [FromServices] IServiceManager serviceManager,
                [FromServices] IValidator<UpdateProjectRequest> validator,
                CancellationToken cancellationToken) =>
            {
                var patchDocument = JsonConvert
                .DeserializeObject<JsonPatchDocument<UpdateProjectRequest>>(
                    jsonElement.GetRawText());

                if (patchDocument is null)
                    return Results.BadRequest("Patch document object is null");

                var (project, updateRequest) = await serviceManager.ProjectService
                .GetProjectForPatchingAsync(id, trackChanges: true, cancellationToken);

                var errors = new List<string>();

                patchDocument.ApplyTo(updateRequest, (error) =>
                {
                    errors.Add(error.ErrorMessage);
                });

                if (errors.Count != 0)
                {
                    var errorsDict = new Dictionary<string, string[]>
                    {
                        {"binding errors",errors.ToArray() }
                    };

                    return Results.ValidationProblem(errorsDict,
                        statusCode: StatusCodes.Status422UnprocessableEntity);
                }

                var validationResult = await validator.ValidateAsync(updateRequest, cancellationToken);

                if (!validationResult.IsValid)
                    return Results.ValidationProblem(
                        validationResult.ToDictionary(),
                        statusCode: StatusCodes.Status422UnprocessableEntity);

                await serviceManager.ProjectService
                .PatchProjectAsync(project, updateRequest, cancellationToken);

                return Results.NoContent();
            })
            .AddEndpointFilter<CacheInvalidationFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithName(ProjectConstants.PatchProject);
        }
    }
}
