using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.CachedValue.Service;

public class LocalMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public LocalMemoryCacheService(
        IMemoryCache memoryCache
    )
    {
        _memoryCache = memoryCache;
    }

    public Task<bool> Create(string key, string value, long expireInSeconds)
    {
        _memoryCache.CreateEntry(key).SetValue(value);
        // await _memoryCache.GetOrCreateAsync(key, (cacheEntry) =>
        // {
        //     cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(expireInSeconds);
        //     return Task.FromResult(value);
        // });
        return Task.FromResult(true);
    }

    public Task<CacheResponse<TEntity>?> Find<TEntity>(CacheRequest request) where TEntity : class
    {
        bool findInCache = _memoryCache.TryGetValue(request.Key, out object? cachedValue);
        if (findInCache == false || cachedValue == null)
        {
            return Task.FromResult<CacheResponse<TEntity>?>(new CacheResponse<TEntity>(request.Key, null));
        }
        string cacheStr = (string)cachedValue;
        TEntity? entity = JsonSerializer.Deserialize<TEntity>(cacheStr);
        return Task.FromResult<CacheResponse<TEntity>?>(new CacheResponse<TEntity>(request.Key, entity));
    }
}