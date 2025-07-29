using Application.Absrtactions;
using Contracts.Subtasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Endpoints.Abstractions;

namespace ProjectManagement.Endpoints
{
    public class SubtasktEndpoints : IMinimalEndpoint
    {
        const string route = "api/projects/{projectId:guid}/subtasks";
        public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
        {

            routeBuilder.MapGet(route, async (
                [FromRoute] Guid projectId,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                var subtastks = await serviceManager.SubtaskService
                .GetAllSubtasksForProjectAsync(projectId, cancellationToken);

                return Results.Ok(subtastks);
            });

            routeBuilder.MapGet(route + "/{id:guid}", async (
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
            })
                .WithName("GetSubtaskById");

            routeBuilder.MapPost(route, async (
                [FromRoute] Guid projectId,
                [FromBody] CreateSubtaskRequest request,
                [FromServices] IServiceManager serviceManager,
                [FromServices] IValidator<CreateSubtaskRequest> validator,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(
                        validationResult.ToDictionary(),
                        statusCode: StatusCodes.Status422UnprocessableEntity);
                }

                var subtask = await serviceManager.SubtaskService
                .CreateSubtaskAsync(projectId, request, cancellationToken);

                return Results.CreatedAtRoute(
                    routeName: "GetSubtaskById",
                    routeValues: new
                    {
                        projectId,
                        id = subtask.Id
                    },
                    value: subtask);
            });
        }
    }
}
