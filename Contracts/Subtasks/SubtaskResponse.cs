using Contracts.Common;

namespace Contracts.Subtasks
{
    public record SubtaskResponse(
        Guid Id,
        string Title,
        string Description,
        bool IsCompleted)
    {
        public List<Link> Links { get; set; } = [];
    }
}
