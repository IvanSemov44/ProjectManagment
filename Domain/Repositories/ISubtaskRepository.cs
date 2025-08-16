using Contracts.Requests;
using Domain.Models;

namespace Domain.Repositories
{
    public interface ISubtaskRepository
    {
        Task<Subtask?> GetSubtaskForProjectAsync(
            Guid projectId,
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken = default);

        Task<PagedList<Subtask>> GetPagedSubtasksForProjectAsync(
            Guid projectId,
            SubtaskRequestParameters requestParams,
            CancellationToken cancellationToken = default);

        void CreateSubtask(Subtask subtask);

        void DeleteSubtask(Subtask subtask);
    }
}
