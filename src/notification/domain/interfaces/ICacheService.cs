using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Domain.Interfaces;

public interface ICacheService
{
    Task<string?> Find(CacheRequest request);
    Task<CacheResponse<TEntity>?> Find<TEntity>(CacheRequest request) where TEntity : class;
    Task<bool> Create(string key, string value, long? expireInSeconds = null);
    Task<string?> DecreaseValue(string key);
    Task<bool> Remove(string key);
}