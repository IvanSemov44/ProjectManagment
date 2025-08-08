using Contracts.Subtasks;
using Domain;

namespace Application.Absrtactions
{
    public interface ISubtaskService
    {
        Task<PagedList<SubtaskResponse>> GetPagedSubtasksForProjectAsync(
            Guid projectId,
            int page,
            int pageSize,
            string? title,
            string? searchTerm,
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

        Task<(Subtask subtast, UpdateSubtaskRequest updateRequest)> GetSubtaskForPatchingAsync(
            Guid projectId,
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken = default);

        Task PatchSubtaskAsync(
            Subtask subtask,
            UpdateSubtaskRequest updateSubtask,
            CancellationToken cancellationToken = default);
    }
}
