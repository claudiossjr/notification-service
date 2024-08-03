using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Notification.Domain.Exceptions.Cache;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Provider.Cache.Local;

public class LocalMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public LocalMemoryCacheService(
        IMemoryCache memoryCache
    )
    {
        _memoryCache = memoryCache;
    }

    public async Task<bool> Create(string key, string value, long? expireIn = null)
    {
        await Task.Yield();
        try
        {
            ICacheEntry cacheEntry = _memoryCache.CreateEntry(key).SetValue(value);
            if (expireIn.HasValue)
            {
                _memoryCache.Set(key, cacheEntry, TimeSpan.FromMilliseconds(expireIn.Value));
            }
            else
            {
                _memoryCache.Set(key, cacheEntry);
            }
            return true;
        }
        catch (Exception ex)
        {
            throw new CacheServerOfflineException($"Local Memory Server {ex.Message}");
        }
    }

    public async Task<string?> DecreaseValue(string key)
    {
        await Task.Yield();
        ICacheEntry? cacheEntry = _memoryCache.Get<ICacheEntry>(key);
        if (cacheEntry == null)
        {
            return null;
        }
        long value = long.Parse(cacheEntry!.Value!.ToString()!);
        value -= 1;
        cacheEntry.Value = value;
        return value.ToString();
    }

    public async Task<CacheResponse<TEntity>?> Find<TEntity>(CacheRequest request) where TEntity : class
    {
        await Task.Yield();
        try
        {
            bool findInCache = _memoryCache.TryGetValue(request.Key, out ICacheEntry? cachedValue);
            if (findInCache == false || cachedValue == null)
            {
                return new CacheResponse<TEntity>(request.Key, null);
            }
            string cacheStr = cachedValue!.Value!.ToString()!;
            TEntity? entity = JsonSerializer.Deserialize<TEntity>(cacheStr);
            return new CacheResponse<TEntity>(request.Key, entity);
        }
        catch (Exception ex)
        {
            throw new CacheServerOfflineException($"Local Memory Server {ex.Message}");
        }
    }

    public async Task<string?> Find(CacheRequest request)
    {
        await Task.Yield();
        _memoryCache.TryGetValue(request.Key, out ICacheEntry? cachedValue);
        return cachedValue?.Value?.ToString();
    }

    public async Task<bool> Remove(string key)
    {
        await Task.Yield();
        try
        {
            _memoryCache.Remove(key);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}