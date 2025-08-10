using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;

namespace ProjectManagement.Policies
{
    public class VaryByIndentifierCachePolicy : IOutputCachePolicy
    {
        public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            var indentifier = context.HttpContext.Request.RouteValues["id"];

            if (indentifier is null)
            {
                return ValueTask.CompletedTask;
            }

            context.Tags.Add(indentifier.ToString()!);

            var meetsRequirements = MeetsOutputCacheRequirements(context);

            context.EnableOutputCaching = true;
            context.AllowCacheLookup = meetsRequirements;
            context.AllowCacheStorage = meetsRequirements;
            context.AllowLocking = true;
            context.CacheVaryByRules.QueryKeys = "*";

            return ValueTask.CompletedTask;

        }

        private static bool MeetsOutputCacheRequirements(OutputCacheContext context)
        {
            var request = context.HttpContext.Request;

            if (!HttpMethods.IsGet(request.Method) &&
                !HttpMethods.IsHead(request.Method))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(request.Headers.Authorization) ||
                request.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                return false;
            }

            return true;
        }

        public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            var response = context.HttpContext.Response;

            if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
            {
                context.AllowCacheStorage = false;
                return ValueTask.CompletedTask;
            }

            if (response.StatusCode != StatusCodes.Status200OK)
            {
                context.AllowCacheStorage = false;
                return ValueTask.CompletedTask;
            }

            return ValueTask.CompletedTask;
        }
    }
}
