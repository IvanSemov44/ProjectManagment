namespace Domain.Models
{
    public class Subtask
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required bool IsCompleted { get; set; }

        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}
