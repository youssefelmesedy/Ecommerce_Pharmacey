using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Pharmacy.Infarstructure.Cacheing;
using System.Collections.Concurrent;

namespace Pharmacy.Infrastructure.Caching.ImplementationMemoryCache;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;

    // 📌 كل prefix مربوط بمجموعة مفاتيح آمنة للخيوط
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _prefixKeys = new();

    public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
    {
        _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region 🔹 GetOrSet
    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        string prefix,
        TimeSpan? duration = null,
        CancellationToken cancellationToken = default)
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue is not null)
        {
            LogSection("CACHE HIT", $"✅ Retrieved from cache\nKey: {key}");
            return cachedValue;
        }

        LogSection("CACHE MISS", $"⚠️ Not found in cache\nKey: {key}");

        var value = await factory();

        await SetAsync(key, value, prefix, duration, cancellationToken);

        LogSection("CACHE SET", $"💾 Cached new value for key: {key} (Duration: {duration ?? TimeSpan.FromMinutes(10)})");

        return value;
    }
    #endregion

    #region 🔹 Get
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            LogSection("CACHE HIT", $"✅ Found value for key: {key}");
            return Task.FromResult(value);
        }

        LogSection("CACHE MISS", $"⚠️ No value found for key: {key}");
        return Task.FromResult(default(T));
    }
    #endregion

    #region 🔹 Set
    public Task SetAsync<T>(
        string key,
        T value,
        string prefix,
        TimeSpan? duration = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration ?? TimeSpan.FromMinutes(10)
            };

            _cache.Set(key, value, options);

            // ✅ أضف المفتاح إلى قاموس الـ prefix بطريقة آمنة
            var innerDict = _prefixKeys.GetOrAdd(prefix, _ => new ConcurrentDictionary<string, byte>());
            innerDict.TryAdd(key, 0);

            _logger.LogDebug($"🟢 Added cache key: {key} under prefix: {prefix}. TotalKeysForPrefix={innerDict.Count}");

            Console.WriteLine($"\n\n_prefix.Count(): {_prefixKeys.Count()}\n\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error setting cache key {key}");
            throw;
        }

        return Task.CompletedTask;
    }
    #endregion

    #region 🔹 Remove
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _cache.Remove(key);

            // شيل المفتاح من أي prefix
            foreach (var kvp in _prefixKeys)
            {
                var prefix = kvp.Key;
                var inner = kvp.Value;

                if (inner.TryRemove(key, out _))
                {
                    if (inner.IsEmpty)
                        _prefixKeys.TryRemove(prefix, out _);

                    _logger.LogDebug($"🗑️ Removed key '{key}' from prefix '{prefix}'");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error removing cache key {key}");
            throw;
        }

        return Task.CompletedTask;
    }
    #endregion

    #region 🔹 Remove By Prefix
    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        try
        {
            Console.WriteLine($"\n\n_prefix.Count(): {_prefixKeys.Count()}\n\n");

            if (_prefixKeys.TryGetValue(prefix, out var keys))
            {
                int count = keys.Count;
                foreach (var key in keys.Keys)
                {
                    _cache.Remove(key);
                    _logger.LogDebug($"🧹 Removed key '{key}' under prefix '{prefix}'");
                }

                keys.Clear();

                LogSection("CACHE REMOVE PREFIX", $"✅ Cleared {count} keys under prefix: {prefix}");
            }
            else
            {
                _logger.LogWarning($"⚠️ No keys found under prefix: {prefix}");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error removing prefix {prefix}");
            throw;
        }

        return Task.CompletedTask;
    }
    #endregion

    #region 🔹 Logger
    private void LogSection(string title, string message)
    {
        _logger.LogInformation($"\n────────────────────────────────────────────\n[{title}]\n{message}\n────────────────────────────────────────────\n");
    }
    #endregion
}
