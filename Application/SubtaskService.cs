using Application.Absrtactions;
using AutoMapper;
using Contracts;
using Contracts.Requests;
using Contracts.Subtasks;
using Domain;
using Domain.Expetions;

namespace Application
{
    public sealed class SubtaskService(ICustomLogger logger, IUnitOfWork unitOfWork, IMapper mapper)
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
    }
}
