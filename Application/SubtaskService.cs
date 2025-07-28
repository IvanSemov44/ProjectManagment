using Application.Absrtactions;
using AutoMapper;
using Contracts;
using Contracts.Subtasks;
using Domain;
using Domain.Expetions;

namespace Application
{
    public sealed class SubtaskService(ICustomLogger logger, IUnitOfWork unitOfWork, IMapper mapper)
        : ISubtaskService
    {
        public async Task<IEnumerable<SubtaskResponse>> GetAllSubtasksForProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var project = await unitOfWork.ProjectRepository
                .GetProjectAsync(projectId, trackChanges: false, cancellationToken)
                ?? throw new ProjectNotFoundException(projectId);


            var subtasks = await unitOfWork.SubtaskRepository
                .GetAllSubtasksForProjectAsync(projectId, cancellationToken);

            var response = mapper.Map<IEnumerable<SubtaskResponse>>(subtasks);

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
    }
}
