using Pharmacy.Infarstructure.Rpositoryies;
using System.Linq.Expressions;

namespace Pharmacy.Infrastructure.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        // Query operations
        Task<TEntity?> FirstOrDefaultAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken);
        Task<IEnumerable<TEntity>> GetAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default);
        Task<TEntity?> GetSingleAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);

        // Existence
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

        // CRUD
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);

        // Queryable Access
        IQueryable<TEntity> AsQueryable();
    }
}
