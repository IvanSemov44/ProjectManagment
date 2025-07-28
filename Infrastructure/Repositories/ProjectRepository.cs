using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class ProjectRepository(ApplicationDbContext context)
        : BaseRepository<Project>(context), IProjectRepository
    {
        public async Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAll().ToListAsync();
        }

        public async Task<Project?> GetProjectAsync(Guid id, bool trackChanges, CancellationToken cancellationToken)
        {
            return await GetByCondition(x => x.Id.Equals(id), trackChanges)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public void CreateProject(Project project)
        {
            Insert(project);
        }

    }
}
