using InventorySys.Application.Common.Models;

namespace InventorySys.Application.Common.Interfaces;

public interface ICacheService
{
    T? Get<T>(CacheKey key);
    Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default);
    void Set<T>(CacheKey key, T value, TimeSpan? absoluteExpirationRelativeToNow = null);
    Task SetAsync<T>(CacheKey key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken cancellationToken = default);
    void Remove(CacheKey key);
    Task<T> GetOrCreateAsync<T>(CacheKey key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
}
