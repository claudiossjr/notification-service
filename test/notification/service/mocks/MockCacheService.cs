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

    public List<string> KeysSearched { get; set; } = [];

    public CacheResponse Find(CacheRequest request)
    {
        CacheCalls++;
        KeysSearched.Add(request.Key);
        if (CanThrowCacheServerOffline) throw new CacheServerOfflineException("mock_cache_service");
        return new(request.Key, string.Empty);
    }
}