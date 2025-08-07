namespace Domain.Repositories
{
    public interface IProjectRepository
    {
        Task<PagedList<Project>> GetPagedProjectsAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Project?> GetProjectAsync(
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken);

        void CreateProject(Project project);

        void DeleteProject(Project project);
    }
}
