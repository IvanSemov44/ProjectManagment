using Application.Absrtactions;
using Contracts;
using Contracts.Subtasks;
using Domain;
using Domain.Expetions;

namespace Application
{
    public sealed class SubtaskService(ICustomLogger logger, IUnitOfWork unitOfWork)
        : ISubtaskService
    {
        public async Task<IEnumerable<SubtaskResponse>> GetAllSubtasksForProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(projectId, trackChanges: false, cancellationToken)
                ?? throw new ProjectNotFoundException(projectId);


            var subtasks = await unitOfWork.SubtaskRepository
                .GetAllSubtasksForProjectAsync(projectId, cancellationToken);

            var response = new List<SubtaskResponse>();

            foreach (var subtask in subtasks)
            {
                response.Add(new SubtaskResponse(
                    Id: subtask.Id,
                    Title: subtask.Title,
                    Description: subtask.Description,
                    IsCompleted: subtask.IsCompleted));
            }

            return response;
        }
    }
}
