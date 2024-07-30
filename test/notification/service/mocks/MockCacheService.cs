using Notification.Domain.Exceptions.Cache;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Service.Test.Mocks;

public class MockCacheService : ICacheService
{

    public bool CanThrowCacheServerOffline { get; set; } = false;

    public int CacheCalls = 0;

    public List<object>? MockReturns { get; set; }

    public List<string> KeysSearched { get; set; } = [];

    public Task<CacheResponse<TEntity>?> Find<TEntity>(CacheRequest request) where TEntity : class
    {
        KeysSearched.Add(request.Key);
        if (CanThrowCacheServerOffline) throw new CacheServerOfflineException("mock_cache_service");

        TEntity result = (TEntity)MockReturns!.ElementAt(CacheCalls);
        CacheCalls++;
        return Task.FromResult<CacheResponse<TEntity>?>(new(request.Key, result));
    }

    public Task<bool> Create(string key, string value, long expireInSeconds)
    {
        return Task.FromResult(true);
    }
}