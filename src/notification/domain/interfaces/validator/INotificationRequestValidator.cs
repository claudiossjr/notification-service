using Notification.Domain.Interfaces.Request;

namespace Notification.Domain.Interfaces.Validator;

public interface INotificationRequestValidator
{
    ValidatorResponse Validate(NotificationRequest request);
}