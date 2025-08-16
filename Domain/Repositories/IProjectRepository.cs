using Contracts.Requests;
using Domain.Models;

namespace Domain.Repositories
{
    public interface IProjectRepository
    {
        Task<PagedList<Project>> GetPagedProjectsAsync(
            ProjectRequestParameters requestParams,
            CancellationToken cancellationToken = default);

        Task<Project?> GetProjectAsync(
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken);

        void CreateProject(Project project);

        void DeleteProject(Project project);
    }
}
