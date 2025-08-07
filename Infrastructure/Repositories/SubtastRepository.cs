using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class SubtastRepository(ApplicationDbContext context)
        : BaseRepository<Subtask>(context), ISubtaskRepository
    {
        public async Task<PagedList<Subtask>> GetPagedSubtasksForProjectAsync(
            Guid projectId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var totalCount = await GetByCondition(x => x.ProjectId.Equals(projectId), trackChanges: false)
                .CountAsync(cancellationToken);

            var pagedSubtask = await GetByCondition(x => x.ProjectId.Equals(projectId), trackChanges: false)
                .OrderBy(x => x.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<Subtask>(pagedSubtask, page, pageSize, totalCount);
        }

        public async Task<Subtask?> GetSubtaskForProjectAsync(Guid projectId, Guid id, bool trackChanges, CancellationToken cancellationToken = default)
        {
            return await GetByCondition(x =>
                x.ProjectId.Equals(projectId) &&
                x.Id.Equals(id),
                trackChanges)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public void CreateSubtask(Subtask subtask)
        {
            Insert(subtask);
        }

        public void DeleteSubtask(Subtask subtask)
        {
            Remove(subtask);
        }
    }
}
