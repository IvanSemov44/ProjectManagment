using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class SubtastRepository(ApplicationDbContext context)
        : BaseRepository<Subtask>(context), ISubtaskRepository
    {
        public async Task<IEnumerable<Subtask>> GetAllSubtasksForProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await GetByCondition(x => x.ProjectId.Equals(projectId), trackChanges: false)
                .OrderBy(x => x.Title)
                .ToListAsync(cancellationToken);
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
    }
}
