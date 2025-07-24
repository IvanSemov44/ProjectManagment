using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public abstract class BaseRepository<T>(ApplicationDbContext context)
        : IBaseRepository<T> where T : class
    {
        public IQueryable<T> GetAll() => context.Set<T>().AsNoTracking();

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
            if (trackChanges)
                return context.Set<T>().Where(expression);

            return context.Set<T>()
                .Where(expression)
                .AsNoTracking();
        }

        public void Insert(T entity) => context.Set<T>().Add(entity);

        public void Remove(T entity) => context.Set<T>().Remove(entity);
    }
}
