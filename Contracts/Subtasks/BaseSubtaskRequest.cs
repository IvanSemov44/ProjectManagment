namespace Contracts.Subtasks
{
    public record BaseSubtaskRequest
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
    }
}
