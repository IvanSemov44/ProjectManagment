using Application.Absrtactions;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManagement.Endpoints
{
    public static class ProjectEndpoints
    {
        public static void RegisterProjectEndpoints(
            this IEndpointRouteBuilder routeBuilder)
        {

            routeBuilder.MapGet("api/projects", async ([
                FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                var projects = await serviceManager.ProjectService
                .GetAllProjectsAsync(cancellationToken);
                return Results.Ok(projects);
            });

            routeBuilder.MapGet("api/projects/{id:guid}", async (
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

        }
    }
}
