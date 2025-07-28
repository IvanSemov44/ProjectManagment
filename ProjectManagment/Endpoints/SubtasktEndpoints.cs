using Application.Absrtactions;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Endpoints.Abstractions;

namespace ProjectManagement.Endpoints
{
    public class SubtasktEndpoints : IMinimalEndpoint
    {
        public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
        {

            routeBuilder.MapGet("api/projects/{projectId:guid}/subtasks", async (
                [FromRoute] Guid projectId,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                var subtastks = await serviceManager.SubtaskService
                .GetAllSubtasksForProjectAsync(projectId, cancellationToken);

                return Results.Ok(subtastks);
            });

            routeBuilder.MapGet("api/projects/{projectId:guid}/subtasks/{id:guid}", async (
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
        }
    }
}
