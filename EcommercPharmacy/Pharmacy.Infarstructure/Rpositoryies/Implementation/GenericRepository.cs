using Microsoft.EntityFrameworkCore;
using Pharmacy.Infarstructure.Rpositoryies;
using Pharmacy.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Pharmacy.Infrastructure.Repositories.Implementations
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        // ✅ Build Query based on QueryOptions
        protected IQueryable<TEntity> BuildQuery(QueryOptions<TEntity> options)
        {
            IQueryable<TEntity> query = _dbSet;

            if (options.FilterExpression is not null)
                query = query.Where(options.FilterExpression);

            foreach (var include in options.Includes)
                query = query.Include(include);

            if (options.OrderBy is not null)
                query = options.OrderBy(query);

            // ✅ GroupBy في قاعدة البيانات
            if (options.GroupBy is not null)
            {
                // هنعمل GroupBy فعليًا في SQL ونرجّع البيانات بشكل مسطّح
                query = query.GroupBy(options.GroupBy)
                             .SelectMany(g => g);
            }

            if (options.Skip.HasValue)
                query = query.Skip(options.Skip.Value);

            if (options.Take.HasValue)
                query = query.Take(options.Take.Value);

            if (options.AsNoTracking)
                query = query.AsNoTracking();

            return query;
        }

        // FirstOrDefaultAsync
        public async Task<TEntity?> FirstOrDefaultAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken)
        {
            var query = BuildQuery(options);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        // ✅ Get multiple entities
        public async Task<IEnumerable<TEntity>> GetAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(options);

            return await query.ToListAsync(cancellationToken);
        }

        // ✅ Get single entity
        public async Task<TEntity?> GetSingleAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(options);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        // ✅ Get by Id
        public async Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
        }

        // ✅ Any & Count
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(predicate, cancellationToken);

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => predicate is null
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(predicate, cancellationToken);

        // ✅ CRUD
        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await _dbSet.AddAsync(entity, cancellationToken);

        public Task Update(TEntity entity)
            => Task.FromResult(_dbSet.Update(entity));

        public Task Delete(TEntity entity)
            => Task.FromResult(_dbSet.Remove(entity));

        // ✅ IQueryable Access
        public IQueryable<TEntity> AsQueryable() => _dbSet.AsQueryable();
    }
}
