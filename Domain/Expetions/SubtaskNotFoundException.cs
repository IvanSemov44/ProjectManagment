using Domain.Expetions.Base;

namespace Domain.Expetions
{
    public sealed class SubtaskNotFoundException(Guid id)
        : NotFoundException($"Subtask with ID: {id} was not found in the database.")
    {
    }
}
