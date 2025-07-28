using Domain.Expetions.Base;

namespace Domain.Expetions
{
    public class ProjectNotFoundException(Guid id)
        : NotFoundException($"Project with ID: {id} was not found in the database.")
    {
    }
}
