namespace Pharmacy.Infarstructure.Cacheing;
public interface ICacheService
{
    Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        string prefix,
        TimeSpan? duration = null,
        CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, string prefix, TimeSpan? duration = null, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}
