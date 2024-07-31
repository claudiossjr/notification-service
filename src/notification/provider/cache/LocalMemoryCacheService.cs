using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Notification.Domain.Exceptions.Cache;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Provider.Cache;

public class LocalMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public LocalMemoryCacheService(
        IMemoryCache memoryCache
    )
    {
        _memoryCache = memoryCache;
    }

    public Task<bool> Create(string key, string value, long? expireInSeconds = null)
    {
        try
        {
            ICacheEntry cacheEntry = _memoryCache.CreateEntry(key).SetValue(value);
            if (expireInSeconds.HasValue)
            {
                _memoryCache.Set(key, cacheEntry, TimeSpan.FromSeconds(expireInSeconds.Value));
            }
            else
            {
                _memoryCache.Set(key, cacheEntry);
            }
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            throw new CacheServerOfflineException($"Local Memory Server {ex.Message}");
        }
    }

    public Task<bool> DecreaseValue(string key)
    {
        ICacheEntry? cacheEntry = _memoryCache.Get<ICacheEntry>(key);
        long value = long.Parse(cacheEntry!.Value!.ToString()!);
        value -= 1;
        cacheEntry.Value = value;
        return Task.FromResult(true);
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

    public Task<string?> Find(CacheRequest request)
    {
        bool findInCache = _memoryCache.TryGetValue(request.Key, out ICacheEntry? cachedValue);
        return Task.FromResult(cachedValue?.Value?.ToString());
    }
}