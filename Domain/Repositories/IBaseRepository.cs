using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IBaseRepository<T>
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetByCondition(
            Expression<Func<T, bool>> expression,
            bool trackChanges);
        void Insert(T entity);
        void Remove(T entity);
    }
}
