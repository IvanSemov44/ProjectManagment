namespace Domain.Repositories
{
    public interface ISubtaskRepository
    {
        Task<Subtask?> GetSubtaskForProjectAsync(
            Guid projectId,
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Subtask>> GetAllSubtasksForProjectAsync(
            Guid projectId,
            CancellationToken cancellationToken = default);
    }
}
