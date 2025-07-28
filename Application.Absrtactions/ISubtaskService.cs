using Contracts.Subtasks;

namespace Application.Absrtactions
{
    public interface ISubtaskService
    {
        Task<IEnumerable<SubtaskResponse>> GetAllSubtasksForProjectAsync(
            Guid projectId,
            CancellationToken cancellationToken = default);
    }
}
