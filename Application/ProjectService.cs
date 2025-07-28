using Application.Absrtactions;
using AutoMapper;
using Contracts;
using Contracts.Projects;
using Domain;
using Domain.Expetions;

namespace Application
{
    public sealed class ProjectService(
        ICustomLogger logger, IUnitOfWork unitOfWork, IMapper mapper)
        : IProjectService
    {
        public async Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(CancellationToken cancellationToken = default)
        {
            var projects = await unitOfWork.ProjectRepository.GetAllProjectsAsync(cancellationToken);

            var response = mapper.Map<IEnumerable<ProjectResponse>>(projects);

            return response;
        }

        public async Task<ProjectResponse> GetProjectAsync(Guid id, bool trackChanges, CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(id, trackChanges, cancellationToken)
                ?? throw new ProjectNotFoundException(id);

            var response = mapper.Map<ProjectResponse>(project);

            return response;
        }
    }
}
