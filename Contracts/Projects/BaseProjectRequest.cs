using Contracts.Subtasks;

namespace Contracts.Projects
{
    public record BaseProjectRequest
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public IEnumerable<CreateSubtaskRequest>? Subtasks { get; init; }
    }
}
