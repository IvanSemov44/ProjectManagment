using Application.Absrtactions;
using Contracts.Subtasks;
using Domain;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectManagement.Endpoints.Abstractions;
using ProjectManagement.Middleware;
using System.Text.Json;

namespace ProjectManagement.Endpoints
{
    public class SubtasktEndpoints : IMinimalEndpoint
    {
        const string route = "api/projects/{projectId:guid}/subtasks";
        const string routeWithSubtaskId = route + "/{id:guid}";
        public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
        {

            routeBuilder.MapGet(route, async (
                [FromRoute] Guid projectId,
                [FromQuery] string? title,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 5) =>
            {
                var subtastks = await serviceManager.SubtaskService
                .GetPagedSubtasksForProjectAsync(projectId, page, pageSize, title, cancellationToken);

                return Results.Ok(subtastks);
            })
            .Produces<PagedList<SubtaskResponse>>();

            routeBuilder.MapGet(routeWithSubtaskId, async (
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
            .Produces<SubtaskResponse>()
            .Produces(StatusCodes.Status404NotFound)
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
            })
            .AddEndpointFilter<ValidationFilter<CreateSubtaskRequest>>()
            .Produces<SubtaskResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);

            routeBuilder.MapPut(routeWithSubtaskId, async (
                [FromRoute] Guid projectId,
                [FromRoute] Guid id,
                [FromBody] UpdateSubtaskRequest request,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken
                ) =>
            {
                await serviceManager.SubtaskService
                .UpdateSubtaskAsync(
                    projectId,
                    id,
                    request,
                    cancellationToken);

                return Results.NoContent();
            })
            .AddEndpointFilter<ValidationFilter<UpdateSubtaskRequest>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);

            routeBuilder.MapDelete(routeWithSubtaskId, async (
                [FromRoute] Guid projectId,
                [FromRoute] Guid id,
                [FromServices] IServiceManager serviceManager,
                CancellationToken cancellationToken) =>
            {
                await serviceManager.SubtaskService
                .DeleteSubtaskAsync(projectId, id, cancellationToken);

                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            routeBuilder.MapPatch(routeWithSubtaskId, async (
                [FromRoute] Guid projectId,
                [FromRoute] Guid id,
                [FromBody] JsonElement jsonElement,
                [FromServices] IServiceManager serviceManager,
                [FromServices] IValidator<UpdateSubtaskRequest> validator,
                CancellationToken cancellationToken) =>
            {
                var patchDocument = JsonConvert
                .DeserializeObject<JsonPatchDocument<UpdateSubtaskRequest>>(
                    jsonElement.GetRawText());

                if (patchDocument is null)
                    return Results.BadRequest("Path document is null");

                var (subtask, updateRequest) = await serviceManager.SubtaskService
                .GetSubtaskForPatchingAsync(
                    projectId,
                    id,
                    trackChanges: true,
                    cancellationToken);


                var errors = new List<string>();

                patchDocument.ApplyTo(updateRequest, (error) =>
                {
                    errors.Add(error.ErrorMessage);
                });

                if (errors.Count != 0)
                {
                    var errorDict = new Dictionary<string, string[]>
                    {
                        { "model-binding errors" , errors.ToArray() }
                    };

                    return Results.ValidationProblem(errorDict,
                        statusCode: StatusCodes.Status422UnprocessableEntity);
                }

                var validationResult = await validator.ValidateAsync(updateRequest, cancellationToken);

                if (!validationResult.IsValid)
                    return Results.ValidationProblem(
                        validationResult.ToDictionary(),
                        statusCode: StatusCodes.Status422UnprocessableEntity);

                await serviceManager.SubtaskService
                .PatchSubtaskAsync(subtask, updateRequest, cancellationToken);

                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
        }
    }
}
