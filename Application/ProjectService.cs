using Application.Absrtactions;
using AutoMapper;
using Contracts;
using Contracts.Projects;
using Contracts.Requests;
using Contracts.Subtasks;
using Domain;
using Domain.Expetions;

namespace Application
{
    public sealed class ProjectService(
        ICustomLogger logger, IUnitOfWork unitOfWork, IMapper mapper, ILinkService linkService)
        : IProjectService
    {
        public async Task<PagedList<ProjectResponse>> GetPagedProjectsAsync(
            ProjectRequestParameters requestParams,
            CancellationToken cancellationToken = default)
        {
            var projects = await unitOfWork.ProjectRepository.GetPagedProjectsAsync(requestParams, cancellationToken);

            var response = mapper.Map<PagedList<ProjectResponse>>(projects);

            GeneratePagedProjectsLinks(response, requestParams);


            return response;
        }


        public async Task<ProjectResponse> GetProjectAsync(Guid id, bool trackChanges, CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(id, trackChanges, cancellationToken)
                ?? throw new ProjectNotFoundException(id);

            var response = mapper.Map<ProjectResponse>(project);

            GenerateProjectLinks(response);

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

        private void GenerateProjectLinks(ProjectResponse response)
        {
            response.Links.Add(
                linkService.GenerateLink(
                    ProjectConstants.GetAllProjects,
                    "projects",
                    "GET",
                    routeValues: new
                    {
                        page = 1,
                        pageSize = 5
                    }));

            response.Links.Add(
                linkService.GenerateLink(
                    ProjectConstants.GetProjectById,
                    "self",
                    "GET",
                    new
                    {
                        id = response.Id
                    }));

            response.Links.Add(
                linkService.GenerateLink(
                    ProjectConstants.CreateProject,
                    "create_project",
                    "POST",
                    null));

            response.Links.Add(
                linkService.GenerateLink(
                    ProjectConstants.UpdateProject,
                    "update_project",
                    "PUT",
                    new
                    {
                        id = response.Id
                    }));

            response.Links.Add(
                linkService.GenerateLink(
                    ProjectConstants.DeleteProject,
                    "delete_project",
                    "DELETE",
                    new
                    {
                        id = response.Id
                    }));

            response.Links.Add(
                linkService.GenerateLink(
                    ProjectConstants.PatchProject,
                    "partially_update_project",
                    "PATCH",
                    new
                    {
                        id = response.Id
                    }));

            response.Links.Add(
                linkService.GenerateLink(
                    SubtaskConstants.GetAllSubtasks,
                    "subtasks",
                    "GET",
                    new
                    {
                        projectId = response.Id,
                        page = 1,
                        pageSize = 5
                    }));
        }

        private void GeneratePagedProjectsLinks(PagedList<ProjectResponse> pagedList, ProjectRequestParameters requestParams)
        {
            foreach(var project in pagedList.Items)
            {
                GenerateProjectLinks(project);
            }

            if (pagedList.HasHextPage)
            {
                pagedList.Links.Add(
                    linkService.GenerateLink(
                        ProjectConstants.GetAllProjects,
                        "next_page",
                        "GET",
                        new
                        {
                            page = requestParams.Page + 1,
                            pageSize = requestParams.PageSize,
                            name = requestParams.Name,
                            searchTerm = requestParams.SearchTerm,
                            sortBy = requestParams.SortBy,
                            sortOrder = requestParams.SortOrder
                        }));
            }

            if (pagedList.HasPreviousPage)
            {
                pagedList.Links.Add(
                    linkService.GenerateLink(
                        ProjectConstants.GetAllProjects,
                        "previous_page",
                        "GET",
                        new
                        {
                            page = requestParams.Page - 1,
                            pageSize = requestParams.PageSize,
                            name = requestParams.Name,
                            searchTerm = requestParams.SearchTerm,
                            sortBy = requestParams.SortBy,
                            sortOrder = requestParams.SortOrder
                        }));
            }
        }
    }
}
