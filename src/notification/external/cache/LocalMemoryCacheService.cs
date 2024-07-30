using System.Net.Cache;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Notification.Domain.Exceptions.Cache;
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

    public Task<bool> Create(string key, string value, long? expireInSeconds)
    {
        try
        {
            ICacheEntry cacheEntry = _memoryCache.CreateEntry(key).SetValue(value);
            if (expireInSeconds.HasValue)
            {
                cacheEntry.SetSlidingExpiration(TimeSpan.FromSeconds(expireInSeconds.Value));
            }
            _memoryCache.Set(key, cacheEntry);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            throw new CacheServerOfflineException($"Local Memory Server {ex.Message}");
        }
    }

    public Task<CacheResponse<TEntity>?> Find<TEntity>(CacheRequest request) where TEntity : class
    {
        try
        {
            bool findInCache = _memoryCache.TryGetValue(request.Key, out ICacheEntry? cachedValue);
            if (findInCache == false || cachedValue == null)
            {
                return Task.FromResult<CacheResponse<TEntity>?>(new CacheResponse<TEntity>(request.Key, null));
            }
            string cacheStr = cachedValue!.Value!.ToString()!;
            TEntity? entity = JsonSerializer.Deserialize<TEntity>(cacheStr);
            return Task.FromResult<CacheResponse<TEntity>?>(new CacheResponse<TEntity>(request.Key, entity));
        }
        catch (Exception ex)
        {
            throw new CacheServerOfflineException($"Local Memory Server {ex.Message}");
        }
    }
}