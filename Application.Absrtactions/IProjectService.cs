using Contracts.Projects;
using Contracts.Requests;
using Domain.Models;

namespace Application.Absrtactions
{
    public interface IProjectService
    {
        Task<PagedList<ShapedEntity>> GetPagedProjectsAsync(
            ProjectRequestParameters requestParams,
            CancellationToken cancellationToken = default);

        Task<ProjectResponse> GetProjectAsync(
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken = default);

        Task<ProjectResponse> CreateProject(
            CreateProjectRequest request,
            CancellationToken cancellationToken = default);

        Task UpdateProjectAsync(
            Guid id,
            UpdateProjectRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteProjectAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<(Project project, UpdateProjectRequest updateProject)> GetProjectForPatchingAsync(
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken = default);

        Task PatchProjectAsync(
            Project project,
            UpdateProjectRequest request,
            CancellationToken cancellationToken = default);
    }
}
