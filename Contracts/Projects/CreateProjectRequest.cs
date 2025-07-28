using Contracts.Subtasks;

namespace Contracts.Projects
{
    public record CreateProjectRequest(
        string Name,
        string Description,
        IEnumerable<CreateSubtaskRequest> Subtasks);
}
