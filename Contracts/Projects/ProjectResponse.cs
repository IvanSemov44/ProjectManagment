using Contracts.Common;

namespace Contracts.Projects
{
    public record ProjectResponse(
        Guid Id,
        string Name,
        string Description)
    {
        public List<Link> Links { get; set; } = [];
    }
}