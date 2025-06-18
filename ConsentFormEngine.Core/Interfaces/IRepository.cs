using ConsentFormEngine.Core.Shared;
using System.Linq.Expressions;

namespace ConsentFormEngine.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> Update(T entity);
        void Delete(T entity);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task SaveChangesAsync();
        IQueryable<T> Query();
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task<PagedList<TDto>> GetPagedAsync<TEntity, TDto>(
            PageRequest request,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, TDto>>? selector = null,
            params Expression<Func<TEntity, object>>[] includes)
            where TEntity : class;

    }
}
