using Contracts.Requests;
using Contracts.Subtasks;
using Domain.Models;

namespace Application.Absrtactions
{
    public interface ISubtaskService
    {
        Task<PagedList<SubtaskResponse>> GetPagedSubtasksForProjectAsync(
            Guid projectId,
            SubtaskRequestParameters requestParams,
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
