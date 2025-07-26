using Contracts.Projects;

namespace Application.Absrtactions
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(
            CancellationToken cancellationToken = default);

        Task<ProjectResponse> GetProjectAsync(
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken = default);
    }
}
