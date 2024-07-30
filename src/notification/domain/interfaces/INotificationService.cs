using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Domain.Interfaces;

public interface INotificationService
{
    Task<NotificationResponse> Notify(NotificationRequest request);
}