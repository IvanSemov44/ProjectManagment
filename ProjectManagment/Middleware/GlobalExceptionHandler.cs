using Contracts;
using Domain.Expetions.Base;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManagement.Middleware
{
    public sealed class GlobalExceptionHandler(
        ICustomLogger logger, IProblemDetailsService problemDetailsService)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError($"The Following exception was thrown: {exception}");

            var problemDetails = exception switch
            {
                NotFoundException => Generate404Response(exception),
                UnauthorizedAccessException => Generate401Response(exception),
                _ => Generate500Response()
            };

            httpContext.Response.StatusCode = (int)problemDetails.Status!;

            return await problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails,
                    Exception = exception,
                });
        }

        private static ProblemDetails Generate401Response(Exception exception) => new()
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-401-unauthorizzed",
            Title = "Unauthorized",
            Status = StatusCodes.Status401Unauthorized,
            Detail = exception.Message
        };

        private static ProblemDetails Generate500Response() => new()
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-500-internal-server-error",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "An exception occurred while processing your request.0"
        };

        private static ProblemDetails Generate404Response(Exception exception) => new()
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-404-not-found",
            Title = "Not Found",
            Status = StatusCodes.Status404NotFound,
            Detail = exception.Message
        };
    }
}
