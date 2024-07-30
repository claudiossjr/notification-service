using Notification.Domain.Exceptions.Cache;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Service.Test.Mocks;

public class MockCacheService : ICacheService
{
    public CacheResponse? MockResponse { get; set; } = null;

    public bool CanThrowCacheServerOffline { get; set; } = false;

    public int CacheCalls = 0;

    public object?[]? MockReturns { get; set; }

    public List<string> KeysSearched { get; set; } = [];

    public Task<TEntity?> Find<TEntity>(CacheRequest request) where TEntity : class
    {
        KeysSearched.Add(request.Key);
        if (CanThrowCacheServerOffline) throw new CacheServerOfflineException("mock_cache_service");

        TEntity? result = (TEntity?)MockReturns!.ElementAt(CacheCalls);
        CacheCalls++;
        return Task.FromResult(result);
    }

    public Task<bool> Create(string key, string value, long expireInSeconds)
    {
        return Task.FromResult(true);
    }
}