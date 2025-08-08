using Application.Absrtactions;
using AutoMapper;
using Contracts;
using Contracts.Projects;
using Contracts.Requests;
using Domain;
using Domain.Expetions;

namespace Application
{
    public sealed class ProjectService(
        ICustomLogger logger, IUnitOfWork unitOfWork, IMapper mapper)
        : IProjectService
    {
        public async Task<PagedList<ProjectResponse>> GetPagedProjectsAsync(
            ProjectRequestParameters requestParams,
            CancellationToken cancellationToken = default)
        {
            var projects = await unitOfWork.ProjectRepository.GetPagedProjectsAsync(requestParams, cancellationToken);

            var response = mapper.Map<PagedList<ProjectResponse>>(projects);

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

        public async Task<ProjectResponse> CreateProject(
            CreateProjectRequest request,
            CancellationToken cancellationToken = default)
        {
            var project = mapper.Map<Project>(request);

            unitOfWork.ProjectRepository.CreateProject(project);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var response = mapper.Map<ProjectResponse>(project);

            return response;
        }

        public async Task UpdateProjectAsync(
            Guid id,
            UpdateProjectRequest request,
            CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(id, trackChanges: true, cancellationToken)
                ?? throw new ProjectNotFoundException(id);

            mapper.Map(request, project);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(id, trackChanges: false, cancellationToken)
                ?? throw new ProjectNotFoundException(id);

            unitOfWork.ProjectRepository.DeleteProject(project);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<(Project project, UpdateProjectRequest updateProject)> GetProjectForPatchingAsync(
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(id, trackChanges, cancellationToken)
                ?? throw new ProjectNotFoundException(id);

            var updateProject = mapper.Map<UpdateProjectRequest>(project);

            return new(project, updateProject);
        }

        public async Task PatchProjectAsync(
            Project project,
            UpdateProjectRequest request,
            CancellationToken cancellationToken = default)
        {
            mapper.Map(request, project);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
