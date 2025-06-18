using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.Core.Shared;
using ConsentFormEngine.DataAccess.Context;
using System.Linq.Expressions;

namespace ConsentFormEngine.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {

        private readonly BaseDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IMapper _mapper;

        public EfRepository(BaseDbContext context, IMapper mapper)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _mapper = mapper;
        }

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
        public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T> AddAsync(T entity)
        {
            var entry = await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entry.Entity; // <--- EF burada ID'yi otomatik atar
        }
        public async Task<T> Update(T entity)
        {
            var entry = _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }
        public void Delete(T entity) => _dbSet.Remove(entity);
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }
        public IQueryable<T> Query() => _context.Set<T>().AsQueryable();
        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
        public async Task<PagedList<TDto>> GetPagedAsync<TEntity, TDto>(
            PageRequest request,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, TDto>>? selector = null,
            params Expression<Func<TEntity, object>>[] includes)
            where TEntity : class
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            foreach (var include in includes)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            if (orderBy != null)
                query = orderBy(query);

            query = query
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize);

            if (selector != null)
            {
                var projected = await query.Select(selector).ToListAsync();
                return new PagedList<TDto>(projected, totalCount, request.PageIndex, request.PageSize);
            }

            var entities = await query.ToListAsync();
            var dtos = _mapper.Map<List<TDto>>(entities);
            return new PagedList<TDto>(dtos, totalCount, request.PageIndex, request.PageSize);
        }





    }
}
