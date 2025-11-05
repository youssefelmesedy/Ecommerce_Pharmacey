using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pharmacy.Infarstructure.Persistens;
using Pharmacy.Infarstructure.UnitOfWorks.Interfaces;
using Pharmacy.Infrastructure.Repositories.Implementations;
using Pharmacy.Infrastructure.Repositories.Interfaces;

namespace Pharmacy.Infarstructure.UnitOfWorks.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContex _dbContext;

    // private cache for repositories
    private readonly Dictionary<Type, object> _repositories = new();

    //// Domain-specific Repositories
    //private IProductRepository? _productRepository;
    //private ICategoryRepository? _categoryRepository;
    //private ISupplierRepository? _supplierRepository;

    public UnitOfWork(ApplicationDbContex dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    // Factory for generic repository
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        if (_repositories.TryGetValue(typeof(TEntity), out var repo))
            return (IGenericRepository<TEntity>)repo;

        var repositoryInstance = new GenericRepository<TEntity>(_dbContext);
        _repositories[typeof(TEntity)] = repositoryInstance;

        return repositoryInstance;
    }

    // Domain-specific repositories
    //public IProductRepository Products => _productRepository ??= new ProductRepository(_dbContext);
    //public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_dbContext);
    //public ISupplierRepository Suppliers => _supplierRepository ??= new SupplierRepository(_dbContext);

    // ✅ SaveChanges
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);

    // ✅ Transactions
    private IDbContextTransaction? _currentTransaction;

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
       Func<CancellationToken, Task<TResult>> action,
       CancellationToken cancellationToken = default)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async ct =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var result = await action(ct);
                await transaction.CommitAsync(ct);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("No transaction started.");

        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    // Dispose pattern
    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        return _dbContext.DisposeAsync();
    }
}
