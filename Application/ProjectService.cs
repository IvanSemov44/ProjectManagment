using Application.Absrtactions;
using Contracts;
using Contracts.Projects;
using Domain;
using Domain.Expetions;

namespace Application
{
    public sealed class ProjectService(ICustomLogger logger, IUnitOfWork unitOfWork)
        : IProjectService
    {
        public async Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(CancellationToken cancellationToken = default)
        {
            var projects = await unitOfWork.ProjectRepository.GetAllProjectsAsync(cancellationToken);

            var response = projects.Select(x => new ProjectResponse(
                Id: x.Id,
                Name: x.Name,
                Description: x.Description))
                .ToList();

            return response;
        }

        public async Task<ProjectResponse> GetProjectAsync(Guid id, bool trackChanges, CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(id, trackChanges, cancellationToken);

            if (project == null)
                throw new ProjectNotFoundException(id);

            var response = new ProjectResponse(
                Id: project.Id,
                Name: project.Name,
                Description: project.Description);

            return response;
        }
    }
}
