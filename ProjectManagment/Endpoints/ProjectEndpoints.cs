using Application.Absrtactions;
using Contracts.Projects;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace ProjectManagement.Endpoints
{
    public static class ProjectEndpoints
    {
        const string route = "api/projects";
        const string routeWithId = route + "/{id:guid}";
        public static void RegisterProjectEndpoints(
            this IEndpointRouteBuilder routeBuilder)
        {

            routeBuilder.MapGet(route, async (
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                var projects = await serviceManager.ProjectService
                .GetAllProjectsAsync(cancellationToken);

                return Results.Ok(projects);
            })
            .Produces<IEnumerable<ProjectResponse>>();

            routeBuilder.MapGet(routeWithId, async (
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
            .Produces<ProjectResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetProjectById");

            routeBuilder.MapPost(route, async (
                [FromBody] CreateProjectRequest request,
                [FromServices] IServiceManager serviceManager,
                [FromServices] IValidator<CreateProjectRequest> validator,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(
                        validationResult.ToDictionary(),
                        statusCode: StatusCodes.Status422UnprocessableEntity);
                }

                var project = await serviceManager.ProjectService
                .CreateProject(request, cancellationToken);

                return Results.CreatedAtRoute(
                    routeName: "GetProjectById",
                    routeValues: new { id = project.Id },
                    value: project);
            })
            .Produces<ProjectResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);

            routeBuilder.MapPut(routeWithId, async (
                [FromRoute] Guid id,
                [FromBody] UpdateProjectRequest request,
                [FromServices] IServiceManager serviceManager,
                [FromServices] IValidator<UpdateProjectRequest> validator,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return Results.ValidationProblem(
                        validationResult.ToDictionary(),
                        statusCode: StatusCodes.Status422UnprocessableEntity);

                await serviceManager.ProjectService
                .UpdateProjectAsync(id, request, cancellationToken);

                return Results.NoContent();
            })
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);

            routeBuilder.MapDelete(routeWithId, async (
                [FromRoute] Guid id,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                await serviceManager.ProjectService
                .DeleteProjectAsync(id, cancellationToken);

                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            routeBuilder.MapPatch(routeWithId, async (
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
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
        }
    }
}
