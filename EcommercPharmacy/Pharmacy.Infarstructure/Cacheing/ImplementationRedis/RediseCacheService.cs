using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Pharmacy.Infarstructure.Cacheing.ImplementationRedis;
public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connection;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> logger)
    {
        _connection = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
    }

    public string BuilderCacheKey(params object?[] parts)
        => string.Join("_", parts.Select(p => p?.ToString() ?? "null"));

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null)
        {
            _logger.LogInformation("✅ Cache hit for key: {Key}", key);
            return cached;
        }

        _logger.LogInformation("⚠️ Cache miss for key: {Key}", key);
        var value = await factory();
        await SetAsync(key, value, duration, cancellationToken);
        return value;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting cache for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, duration);
            _logger.LogInformation("💾 Cache set for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error setting cache for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogInformation("🧹 Cache removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error removing cache for key: {Key}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoints = _connection.GetEndPoints();
            var server = _connection.GetServer(endpoints.First());
            int removedCount = 0;

            // نستخدم SCAN للبحث عن المفاتيح اللي تبدأ بالـ prefix
            foreach (var key in server.Keys(pattern: $"{prefix}*"))
            {
                await _database.KeyDeleteAsync(key);
                removedCount++;
                _logger.LogDebug("Removed key: {Key} under prefix: {Prefix}", key, prefix);
            }

            if (removedCount > 0)
                _logger.LogInformation("🧹 Removed {Count} keys under prefix: {Prefix}", removedCount, prefix);
            else
                _logger.LogInformation("⚠️ No keys found under prefix: {Prefix}", prefix);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error removing keys with prefix: {Prefix}", prefix);
        }
    }

    public Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, string prefix, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<T>(string key, T value, string prefix, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
