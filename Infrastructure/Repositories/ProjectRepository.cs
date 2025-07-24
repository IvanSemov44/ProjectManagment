using Domain;
using Domain.Repositories;

namespace Infrastructure.Repositories
{
    public sealed class ProjectRepository(ApplicationDbContext context)
        : BaseRepository<Project>(context), IProjectRepository
    {
    }
}
