using Contracts.Requests;
using Domain.Models;
using Domain.Repositories;
using Infrastructure.Repositories.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class SubtastRepository(ApplicationDbContext context)
        : BaseRepository<Subtask>(context), ISubtaskRepository
    {
        public async Task<PagedList<Subtask>> GetPagedSubtasksForProjectAsync(
            Guid projectId,
            SubtaskRequestParameters requestParams,
            CancellationToken cancellationToken = default)
        {
            var query = GetByCondition(x => x.ProjectId.Equals(projectId), trackChanges: false)
                .Filter(requestParams.Title)
                .Search(requestParams.SearchTerm)
                .Sort(requestParams.SortBy, requestParams.SortOrder);

            var totalCount = await query.CountAsync(cancellationToken);

            var pagedSubtask = await query
                .Skip((requestParams.Page - 1) * requestParams.PageSize)
                .Take(requestParams.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<Subtask>(pagedSubtask, requestParams.Page, requestParams.PageSize, totalCount);
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
