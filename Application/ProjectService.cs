using Application.Absrtactions;
using AutoMapper;
using Contracts;
using Contracts.Common;
using Contracts.Projects;
using Contracts.Requests;
using Contracts.Subtasks;
using Domain;
using Domain.Expetions;
using Domain.Models;

namespace Application
{
    public sealed class ProjectService(
        ICustomLogger logger,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILinkService linkService,
        IDataShapingService<ProjectResponse> dataShapingService)
        : IProjectService
    {
        public async Task<PagedList<ShapedEntity>> GetPagedProjectsAsync(
            ProjectRequestParameters requestParams,
            CancellationToken cancellationToken = default)
        {
            var projects = await unitOfWork.ProjectRepository.GetPagedProjectsAsync(requestParams, cancellationToken);

            var response = mapper.Map<IEnumerable<ProjectResponse>>(projects.Items);

            var shapedResponse = dataShapingService
                .ShapeData(response, requestParams.Properties!)
                .ToList();

            var pagedResponse = new PagedList<ShapedEntity>(
                shapedResponse,
                projects.Page,
                projects.PageSize,
                projects.TotalCount);

            GeneratePagedProjectsLinks(pagedResponse, requestParams);

            return pagedResponse;
        }


        public async Task<ProjectResponse> GetProjectAsync(Guid id, bool trackChanges, CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(id, trackChanges, cancellationToken)
                ?? throw new ProjectNotFoundException(id);

            var response = mapper.Map<ProjectResponse>(project);

            response.Links = GenerateProjectLinks(response.Id).ToList();

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

        private IEnumerable<Link> GenerateProjectLinks(Guid id)
        {
            var links = new List<Link>
            {
                linkService.GenerateLink(
                    ProjectConstants.GetAllProjects,
                    "projects",
                    "GET",
                    routeValues: new
                    {
                        page = 1,
                        pageSize = 5
                    }),

                linkService.GenerateLink(
                    ProjectConstants.GetProjectById,
                    "self",
                    "GET",
                    new { id }),

                linkService.GenerateLink(
                    ProjectConstants.CreateProject,
                    "create_project",
                    "POST",
                    null),

                linkService.GenerateLink(
                    ProjectConstants.UpdateProject,
                    "update_project",
                    "PUT",
                    new { id }),

                linkService.GenerateLink(
                    ProjectConstants.DeleteProject,
                    "delete_project",
                    "DELETE",
                    new { id }),

                linkService.GenerateLink(
                    ProjectConstants.PatchProject,
                    "partially_update_project",
                    "PATCH",
                    new { id }),

                linkService.GenerateLink(
                    SubtaskConstants.GetAllSubtasks,
                    "subtasks",
                    "GET",
                    new
                    {
                        projectId = id,
                        page = 1,
                        pageSize = 5
                    }),
            };

            return links;
        }

        private void GeneratePagedProjectsLinks(
            PagedList<ShapedEntity> pagedList,
            ProjectRequestParameters requestParams)
        {
            foreach (var project in pagedList.Items)
            {

                var links = GenerateProjectLinks(project.Id);
                ((IDictionary<string, object?>)project.Entity)["Links"] = links;
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
