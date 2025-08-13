using Application.Absrtactions;
using Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Endpoints.Abstractions;
using ProjectManagement.Middleware;

namespace ProjectManagement.Endpoints
{
    public class AuthenticationEndpoints : IMinimalEndpoint
    {
        public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
        {
            var group = routeBuilder.MapGroup("auth")
                .WithTags("Authentication");

            group.MapPost("register", async (
                [FromBody] RegisterUserRequest user,
                [FromServices] IServiceManager serviceManager) =>
            {
                var result = await serviceManager.AuthenticationService
                .RegisterUserAsync(user);

                if (!result.Succeeded)
                {
                    var errorsDict = new Dictionary<string, string[]>
                    {
                        {
                            "user-validation errors",
                            result.Errors.Select(x=>x.Description).ToArray()
                        }
                    };
                    return Results.ValidationProblem(errorsDict);
                }

                return Results.Created();
            })
                .AddEndpointFilter<ValidationFilter<RegisterUserRequest>>()
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
        }
    }
}
