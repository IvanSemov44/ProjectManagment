using Contracts.Subtasks;

namespace Contracts.Projects
{
    public record UpdateProjectRequest(
        string Name,
        string Description,
        IEnumerable<CreateSubtaskRequest> Subtasks);
}
