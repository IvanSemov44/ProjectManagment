namespace Domain.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public required string Description { get; set; }
        public ICollection<Subtask> Subtasks { get; set; } = [];
    }
}
