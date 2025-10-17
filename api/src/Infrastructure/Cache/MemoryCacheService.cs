using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace InventorySys.Infrastructure.Cache;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public T? Get<T>(CacheKey key)
        {
            if (_memoryCache.TryGetValue(key.Value, out T? value))
            {
                CacheServiceLog.CacheHit(_logger, key.Value);
                return value;
            }

            CacheServiceLog.CacheMiss(_logger, key.Value);
            return default;
        }

        public Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default)
            => Task.FromResult(Get<T>(key));

        public void Set<T>(CacheKey key, T value, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(120)
            };

            _memoryCache.Set(key.Value, value, options);
            CacheServiceLog.CacheSet(_logger, key.Value);
        }

        public Task SetAsync<T>(CacheKey key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken cancellationToken = default)
        {
            Set(key, value, absoluteExpirationRelativeToNow);
            return Task.CompletedTask;
        }

        public void Remove(CacheKey key)
        {
            _memoryCache.Remove(key.Value);
            CacheServiceLog.CacheInvalidated(_logger, key.Value);
        }

        public async Task<T> GetOrCreateAsync<T>(CacheKey key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            if (_memoryCache.TryGetValue(key.Value, out T? cachedValue))
            {
                CacheServiceLog.CacheHit(_logger, key.Value);
                return cachedValue!;
            }

            CacheServiceLog.CacheRebuild(_logger, key.Value);

            var newValue = await factory();
            Set(key, newValue, expiration);
            return newValue;
        }
}

// Source Generator Logging
internal static partial class CacheServiceLog
    {
        [LoggerMessage(EventId = 1100, Level = LogLevel.Debug, Message = "Cache hit: {Key}")]
        public static partial void CacheHit(ILogger logger, string key);

        [LoggerMessage(EventId = 1101, Level = LogLevel.Debug, Message = "Cache miss: {Key}")]
        public static partial void CacheMiss(ILogger logger, string key);

        [LoggerMessage(EventId = 1102, Level = LogLevel.Information, Message = "Cache set: {Key}")]
        public static partial void CacheSet(ILogger logger, string key);

        [LoggerMessage(EventId = 1103, Level = LogLevel.Information, Message = "Cache invalidated: {Key}")]
        public static partial void CacheInvalidated(ILogger logger, string key);

        [LoggerMessage(EventId = 1104, Level = LogLevel.Information, Message = "Cache rebuild: {Key}")]
        public static partial void CacheRebuild(ILogger logger, string key);
    }