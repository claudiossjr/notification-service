using Notification.Domain.Exceptions.Cache;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Service.Test.Mocks;

public class MockCacheService : ICacheService
{

    public bool CanThrowCacheServerOffline { get; set; } = false;

    public int CacheFindCalls { get; set; } = 0;
    public int CacheCreateCalls { get; set; } = 0;
    public int CacheDecreaseCalls { get; set; } = 0;

    public Dictionary<string, object?>? MockReturns { get; set; }

    public List<string> KeysSearched { get; set; } = [];

    public Task<CacheResponse<TEntity>?> Find<TEntity>(CacheRequest request) where TEntity : class
    {
        KeysSearched.Add(request.Key);
        if (CanThrowCacheServerOffline) throw new CacheServerOfflineException("mock_cache_service");
        MockReturns!.TryGetValue(request.Key, out object? dictValue);
        TEntity? result = (TEntity?)dictValue;
        CacheFindCalls++;
        return Task.FromResult<CacheResponse<TEntity>?>(new(request.Key, result));
    }

    public Task<bool> Create(string key, string value, long? expireInSeconds = null)
    {
        CacheCreateCalls++;
        return Task.FromResult(true);
    }

    public Task<string?> Find(CacheRequest request)
    {
        KeysSearched.Add(request.Key);
        CacheFindCalls++;
        return Task.FromResult(MockReturns?[request.Key]?.ToString());
    }

    async Task<string?> ICacheService.DecreaseValue(string key)
    {
        CacheDecreaseCalls++;
        string? returnValue = await Find(new CacheRequest(key));
        return returnValue;
    }
}