using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class ProjectRepository(ApplicationDbContext context)
        : BaseRepository<Project>(context), IProjectRepository
    {
        public async Task<PagedList<Project>> GetPagedProjectsAsync(
            int page,
            int pageSize,
            string? name,
            CancellationToken cancellationToken = default)
        {
            var query = GetAll();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var pagedProjects = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<Project>(pagedProjects, page, pageSize, totalCount);
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

        public void DeleteProject(Project project)
        {
            Remove(project);
        }
    }
}