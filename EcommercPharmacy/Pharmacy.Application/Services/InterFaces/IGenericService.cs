using Pharmacy.Infarstructure.Rpositoryies;
using System.Linq.Expressions;

namespace Pharmacy.Infarstructure.Services.InterFaces;
public interface IGenericService<TEntity> where TEntity : class
{
    // 🔹 Query operations
    Task<IEnumerable<TEntity>> GetAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default);
    Task<TEntity?> GetSingleAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);

    // 🔹 Existence & Count
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    // 🔹 CRUD
    Task<int> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
