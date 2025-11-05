using Microsoft.Extensions.Logging;
using Pharmacy.Infarstructure.Cacheing;
using Pharmacy.Infarstructure.Rpositoryies;
using Pharmacy.Infarstructure.Services.InterFaces;
using Pharmacy.Infarstructure.UnitOfWorks.Interfaces;
using System.Linq.Expressions;

namespace Pharmacy.Application.Services.Implementation;
public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : class
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly ICacheService _cache;
    protected readonly ILogger<GenericService<TEntity>> _logger;
    protected readonly string _cachePrefix;

    public GenericService(IUnitOfWork unitOfWork, ICacheService cache, ILogger<GenericService<TEntity>> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cachePrefix = typeof(TEntity).Name ;
    }

    // 🟢 Query
    public async Task<IEnumerable<TEntity>> GetAsync(QueryOptions<TEntity> queryOptions, CancellationToken cancellationToken = default)
    {
        string key = CacheKeyBuilder.BuilderCacheKey(_cachePrefix, "GetAsync", queryOptions);

        try
        {
            var dataInCache = await _cache.GetAsync<IEnumerable<TEntity>>(key, cancellationToken);
            if(dataInCache is not null)
            {
                _logger.LogInformation($"✅ Loaded {typeof(TEntity).Name} list from Caching Memory.");
                return dataInCache;
            }

            var data = await _unitOfWork.Repository<TEntity>().GetAsync(queryOptions, cancellationToken);

            _logger.LogInformation($"✅ Loaded {typeof(TEntity).Name} list from database.");

            await _cache.SetAsync(key, data, _cachePrefix, TimeSpan.FromMinutes(5), cancellationToken);

            return data ?? new List<TEntity>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error fetching {typeof(TEntity).Name} list.");
            throw;
        }
    }

    public async Task<TEntity?> GetSingleAsync(QueryOptions<TEntity> queryOptions, CancellationToken cancellationToken = default)
    {
        string key = CacheKeyBuilder.BuilderCacheKey(_cachePrefix, "GetSingle", queryOptions);

        return await _cache.GetOrSetAsync(key, async () =>
        {
            try
            {
                return await _unitOfWork.Repository<TEntity>().GetSingleAsync(queryOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error fetching single {typeof(TEntity).Name}");
                throw;
            }
        }, _cachePrefix, TimeSpan.FromMinutes(5), cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
    {
        string key = CacheKeyBuilder.BuilderCacheKey(_cachePrefix, "ById", id);

        return await _cache.GetOrSetAsync(key, async () =>
        {
            try
            {
                return await _unitOfWork.Repository<TEntity>().GetByIdAsync(id, cancellationToken);        
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error fetching {typeof(TEntity).Name} by ID {id}");
                throw;
            }
        }, _cachePrefix, TimeSpan.FromMinutes(10), cancellationToken);
    }

    // 🔍 Any / Count
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<TEntity>().AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error checking Any() for {typeof(TEntity).Name}");
            throw;
        }
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Repository<TEntity>().CountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error counting {typeof(TEntity).Name}");
            throw;
        }
    }

    // ✳️ CRUD
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.Repository<TEntity>().AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveByPrefixAsync(_cachePrefix, cancellationToken);

            _logger.LogInformation($"🟢 Added new {typeof(TEntity).Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error adding {typeof(TEntity).Name}");
            throw;
        }
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.Repository<TEntity>().Update(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveByPrefixAsync(_cachePrefix, cancellationToken);
            _logger.LogInformation($"🟡 Updated {typeof(TEntity).Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error updating {typeof(TEntity).Name}");
            throw;
        }
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.Repository<TEntity>().Delete(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveByPrefixAsync(_cachePrefix, cancellationToken);

            _logger.LogInformation($"🔴 Deleted {typeof(TEntity).Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error deleting {typeof(TEntity).Name}");
            throw;
        }
    }
}
