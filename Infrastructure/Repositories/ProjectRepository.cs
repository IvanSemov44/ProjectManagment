using Contracts.Requests;
using Domain.Models;
using Domain.Repositories;
using Infrastructure.Repositories.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class ProjectRepository(ApplicationDbContext context)
        : BaseRepository<Project>(context), IProjectRepository
    {
        public async Task<PagedList<Project>> GetPagedProjectsAsync(
            ProjectRequestParameters requestParams,
            CancellationToken cancellationToken = default)
        {
            var query = GetAll()
                .Filter(requestParams.Name)
                .Search(requestParams.SearchTerm)
                .Sort(requestParams.SortBy, requestParams.SortOrder);

            var totalCount = await query.CountAsync(cancellationToken);

            var pagedProjects = await query
                .Skip((requestParams.Page - 1) * requestParams.PageSize)
                .Take(requestParams.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<Project>(
                pagedProjects,
                requestParams.Page,
                requestParams.PageSize,
                totalCount);
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