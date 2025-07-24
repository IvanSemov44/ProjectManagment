using Domain;
using Domain.Repositories;

namespace Infrastructure.Repositories
{
    public sealed class SubtastRepository(ApplicationDbContext context)
        : BaseRepository<Subtask>(context), ISubtaskRepository
    {
    }
}
