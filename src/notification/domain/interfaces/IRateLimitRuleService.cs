using Notification.Domain.Entites;

namespace Notification.Domain.Interfaces;

public interface IRateLimitRuleService
{
    Task<NotificationRule?> FindByKey(string key);
    Task<bool> Create(NotificationRule rule);
    Task<bool> Delete(string key);
    Task<bool> Update(NotificationRule rule);
}
