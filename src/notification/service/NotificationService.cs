using Notification.Domain.Enums;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;
using Notification.Domain.Interfaces.Validator;

namespace Notification.Service;

public class NotificationService : INotificationService
{

    private readonly INotificationRequestValidator _notificationRequestValidator;

    public NotificationService(INotificationRequestValidator notificationRequestValidator)
    {
        _notificationRequestValidator = notificationRequestValidator;
    }

    public NotificationResponse Notify(NotificationRequest request)
    {
        ValidatorResponse validatorResponse = _notificationRequestValidator.Validate(request);
        if (validatorResponse.IsValid == false)
        {
            return new(validatorResponse.IsValid, NotificationResponseCode.BadRequest, validatorResponse.ErrorMessage);
        }

        return new(false, NotificationResponseCode.Notfound, $"There is no rule for the sender: {request.Sender}");

    }
}
