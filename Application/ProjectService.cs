using Application.Absrtactions;
using Contracts;
using Contracts.Projects;
using Domain;

namespace Application
{
    public sealed class ProjectService(ICustomLogger logger, IUnitOfWork unitOfWork)
        : IProjectService
    {
        public async Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var projects = await unitOfWork.ProjectRepository.GetAllProjectsAsync(cancellationToken);

                var response = projects.Select(x => new ProjectResponse(
                    Id: x.Id,
                    Name: x.Name,
                    Description: x.Description))
                    .ToList();

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"The exception was trown by the {nameof(GetAllProjectsAsync)}");
                throw;
            }
        }

        public async Task<ProjectResponse> GetProjectAsync(Guid id, bool trackChanges, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.ProjectRepository
                    .GetProjectAsync(id, trackChanges, cancellationToken);

                var response = new ProjectResponse(
                    Id: project.Id,
                    Name: project.Name,
                    Description: project.Description);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"The exception was thrown by the {nameof(GetProjectAsync)} method: {ex}");

                throw;
            }
        }
    }
}
