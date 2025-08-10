using Contracts.Projects;
using Contracts.Subtasks;
using Microsoft.AspNetCore.OutputCaching;

namespace ProjectManagement.Middleware
{
    public class CacheInvalidationFilter(IOutputCacheStore cacheStore) : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var result = await next(context);

            var cancellationToken = context.HttpContext.RequestAborted;
            var indentifier = context.HttpContext.Request.RouteValues["id"];

            if (indentifier is not null)
            {
                await cacheStore.EvictByTagAsync(indentifier.ToString()!, cancellationToken);
            }

            var path = context.HttpContext.Request.Path.Value;

            if (path is not null && path.Contains("subtasks"))
            {
                await cacheStore.EvictByTagAsync(SubtaskConstants.GetAllSubtasks, cancellationToken);
            }
            else
            {
                await cacheStore.EvictByTagAsync(ProjectConstants.GetAllProjects, cancellationToken);
            }

            return result;
        }
    }
}
