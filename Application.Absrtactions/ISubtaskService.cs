using Contracts.Subtasks;

namespace Application.Absrtactions
{
    public interface ISubtaskService
    {
        Task<IEnumerable<SubtaskResponse>> GetAllSubtasksForProjectAsync(
            Guid projectId,
            CancellationToken cancellationToken = default);

        Task<SubtaskResponse> GetSubtaskForProjectAsync(
            Guid projectId,
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken = default);

        Task<SubtaskResponse> CreateSubtaskAsync(
            Guid projectId,
            CreateSubtaskRequest request,
            CancellationToken cancellationToken);

        Task UpdateSubtaskAsync(
            Guid projectId,
            Guid id,
            UpdateSubtaskRequest request,
            CancellationToken cancellationToken);

        Task DeleteSubtaskAsync(
            Guid projectId,
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
