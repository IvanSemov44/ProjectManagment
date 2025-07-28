namespace Domain.Repositories
{
    public interface ISubtaskRepository
    {
        Task<IEnumerable<Subtask>> GetAllSubtasksForProjectAsync(
            Guid projectId,
            CancellationToken cancellationToken = default);
    }
}
