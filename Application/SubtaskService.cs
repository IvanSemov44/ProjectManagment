using Application.Absrtactions;
using AutoMapper;
using Contracts;
using Contracts.Projects;
using Contracts.Requests;
using Contracts.Subtasks;
using Domain;
using Domain.Expetions;
using Domain.Models;

namespace Application
{
    public sealed class SubtaskService(ICustomLogger logger, IUnitOfWork unitOfWork, IMapper mapper, ILinkService linkService)
        : ISubtaskService
    {
        public async Task<PagedList<SubtaskResponse>> GetPagedSubtasksForProjectAsync(
            Guid projectId,
            SubtaskRequestParameters requestParams,
            CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(projectId, trackChanges: false, cancellationToken)
                ?? throw new ProjectNotFoundException(projectId);


            var subtasks = await unitOfWork.SubtaskRepository
                .GetPagedSubtasksForProjectAsync(projectId, requestParams, cancellationToken);

            var response = mapper.Map<PagedList<SubtaskResponse>>(subtasks);

            GeneratePagedSubtasksLinks(response, requestParams,projectId);

            return response;
        }

        public async Task<SubtaskResponse> GetSubtaskForProjectAsync(Guid projectId, Guid id, bool trackChanges, CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(projectId, trackChanges, cancellationToken)
                ?? throw new ProjectNotFoundException(projectId);

            var subtask = await unitOfWork.SubtaskRepository
                .GetSubtaskForProjectAsync(projectId, id, trackChanges, cancellationToken)
                ?? throw new SubtaskNotFoundException(id);

            var response = mapper.Map<SubtaskResponse>(subtask);

            GenerateSubtaskLinks(response, projectId);

            return response;
        }

        public async Task<SubtaskResponse> CreateSubtaskAsync(
            Guid projectId,
            CreateSubtaskRequest request,
            CancellationToken cancellationToken)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(projectId, trackChanges: false, cancellationToken)
                ?? throw new ProjectNotFoundException(projectId);

            var subtask = mapper.Map<Subtask>(request);
            subtask.ProjectId = projectId;

            unitOfWork.SubtaskRepository.CreateSubtask(subtask);
            await unitOfWork.SaveChangesAsync();

            var response = mapper.Map<SubtaskResponse>(subtask);

            return response;
        }

        public async Task UpdateSubtaskAsync(
            Guid projectId,
            Guid id,
            UpdateSubtaskRequest request,
            CancellationToken cancellationToken)
        {
            _ = await unitOfWork.ProjectRepository
                .GetProjectAsync(projectId, trackChanges: false, cancellationToken)
                ?? throw new ProjectNotFoundException(projectId);

            var subtask = await unitOfWork.SubtaskRepository
                .GetSubtaskForProjectAsync(projectId, id, trackChanges: true, cancellationToken)
                ?? throw new SubtaskNotFoundException(id);

            mapper.Map(request, subtask);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSubtaskAsync(
            Guid projectId,
            Guid id,
            CancellationToken cancellationToken)
        {
            _ = await unitOfWork.ProjectRepository
                .GetProjectAsync(projectId, trackChanges: false, cancellationToken)
                ?? throw new ProjectNotFoundException(projectId);

            var subtask = await unitOfWork.SubtaskRepository
                .GetSubtaskForProjectAsync(projectId, id, trackChanges: false, cancellationToken)
                ?? throw new SubtaskNotFoundException(id);

            unitOfWork.SubtaskRepository.DeleteSubtask(subtask);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<(Subtask subtast, UpdateSubtaskRequest updateRequest)> GetSubtaskForPatchingAsync(
            Guid projectId,
            Guid id,
            bool trackChanges,
            CancellationToken cancellationToken = default)
        {
            _ = await unitOfWork.ProjectRepository
                .GetProjectAsync(projectId, trackChanges: false, cancellationToken)
                ?? throw new ProjectNotFoundException(projectId);

            var subtask = await unitOfWork.SubtaskRepository
                .GetSubtaskForProjectAsync(projectId, id, trackChanges, cancellationToken)
                ?? throw new SubtaskNotFoundException(id);

            var updateRequest = mapper.Map<UpdateSubtaskRequest>(subtask);

            return new(subtask, updateRequest);
        }

        public async Task PatchSubtaskAsync(Subtask subtask, UpdateSubtaskRequest updateSubtask, CancellationToken cancellationToken = default)
        {
            mapper.Map(updateSubtask, subtask);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private void GenerateSubtaskLinks(SubtaskResponse response, Guid projectId)
        {
            response.Links.Add(
                linkService.GenerateLink(
                    SubtaskConstants.GetAllSubtasks,
                    "subtasks",
                    "GET",
                    routeValues: new
                    {
                        projectId = projectId,
                        page = 1,
                        pageSize = 5
                    }));

            response.Links.Add(
                linkService.GenerateLink(
                    SubtaskConstants.GetSubtaskById,
                    "self",
                    "GET",
                    new
                    {
                        projectId = projectId,
                        id = response.Id,
                    }));

            response.Links.Add(
                linkService.GenerateLink(
                    SubtaskConstants.CreateSubtask,
                    "create_subtask",
                    "POST",
                    null));

            response.Links.Add(
                linkService.GenerateLink(
                    SubtaskConstants.UpdateSubtask,
                    "update_subtask",
                    "PUT",
                    new
                    {
                        projectId = projectId,
                        id = response.Id,
                    }));

            response.Links.Add(
                linkService.GenerateLink(
                    SubtaskConstants.DeleteSubtask,
                    "delete_subtask",
                    "DELETE",
                    new
                    {
                        projectId = projectId,
                        id = response.Id,
                    }));

            response.Links.Add(
                linkService.GenerateLink(
                    SubtaskConstants.PatchSubtask,
                    "partially_update_project",
                    "PATCH",
                    new
                    {
                        projectId = projectId,
                        id = response.Id,
                    }));
        }

        private void GeneratePagedSubtasksLinks(PagedList<SubtaskResponse> pagedList, SubtaskRequestParameters requestParameters, Guid projectId)
        {
            foreach (var subtask in pagedList.Items)
            {
                GenerateSubtaskLinks(subtask, projectId);
            }

            if (pagedList.HasHextPage)
            {
                pagedList.Links.Add(
                    linkService.GenerateLink(
                        SubtaskConstants.GetAllSubtasks,
                        "next_page",
                        "GET",
                        new
                        {
                            projectId = projectId,
                            page = requestParameters.Page,
                            pageSize = requestParameters.PageSize,
                            title = requestParameters.Title,
                            searchTerm = requestParameters.SearchTerm,
                            sortBy = requestParameters.SortBy,
                            sortOrder = requestParameters.SortOrder,
                        }));
            }

            if (pagedList.HasPreviousPage)
            {
                pagedList.Links.Add(
                    linkService.GenerateLink(
                        SubtaskConstants.GetAllSubtasks,
                        "previous_page",
                        "GET",
                        new
                        {
                            projectId = projectId,
                            page = requestParameters.Page,
                            pageSize = requestParameters.PageSize,
                            title = requestParameters.Title,
                            searchTerm = requestParameters.SearchTerm,
                            sortBy = requestParameters.SortBy,
                            sortOrder = requestParameters.SortOrder
                        }));
            }
        }
    }
}
