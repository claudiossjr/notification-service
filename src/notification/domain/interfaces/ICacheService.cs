using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Domain.Interfaces;

public interface ICacheService
{
    CacheResponse Find(CacheRequest request);
}