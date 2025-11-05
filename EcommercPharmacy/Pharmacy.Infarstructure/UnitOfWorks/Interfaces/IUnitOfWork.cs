using Microsoft.EntityFrameworkCore.Storage;
using Pharmacy.Infrastructure.Repositories.Interfaces;

namespace Pharmacy.Infarstructure.UnitOfWorks.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Generic Repository Factory
    /// </summary>
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

    /// <summary>
    /// Domain-specific repositories
    /// </summary>
    //IProductRepository Products { get; }
    //ICategoryRepository Categories { get; }
    //ISupplierRepository Suppliers { get; }

    /// <summary>
    /// Transactions
    /// </summary>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
