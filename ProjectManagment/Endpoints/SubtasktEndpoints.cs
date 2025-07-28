using Application.Absrtactions;
using Contracts.Subtasks;
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
                CancellationToken cancellationToken) =>
            {
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
