namespace Domain.Repositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync(
            CancellationToken cancellationToken = default);

        Task<Project?> GetProjectAsync(
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken);

        void CreateProject(Project project);
    }
}
