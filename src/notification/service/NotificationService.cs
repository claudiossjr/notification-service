using Notification.Domain.Enums;
using Notification.Domain.Exceptions.Cache;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;
using Notification.Domain.Interfaces.Validator;

namespace Notification.Service;

public class NotificationService : INotificationService
{

    private readonly INotificationRequestValidator _notificationRequestValidator;
    private readonly ICacheService _cacheService;

    public NotificationService(
        INotificationRequestValidator notificationRequestValidator,
        ICacheService cacheService
    )
    {
        _notificationRequestValidator = notificationRequestValidator;
        _cacheService = cacheService;
    }

    public NotificationResponse Notify(NotificationRequest request)
    {
        ValidatorResponse validatorResponse = _notificationRequestValidator.Validate(request);
        if (validatorResponse.IsValid == false)
        {
            return new(validatorResponse.IsValid, NotificationResponseCode.BadRequest, validatorResponse.ErrorMessage);
        }

        CacheResponse? senderRule = null;
        try
        {
            senderRule = _cacheService.Find(new($"Rule:{request.Sender}"));
        }
        catch (CacheServerOfflineException)
        {
            // TODO: log error message
            return new(false, NotificationResponseCode.FailedDependency, $"Can not send notification message");
        }
        CacheResponse? recipientTokenBucket = null;
        try
        {
            recipientTokenBucket = _cacheService.Find(new($"Bucket:{request.Recipient}"));
        }
        catch (CacheServerOfflineException)
        {
            // TODO: log error message
            return new(false, NotificationResponseCode.FailedDependency, $"Can not send notification message");
        }

        return new(false, NotificationResponseCode.Notfound, $"There is no rule for the sender: {request.Sender}");

    }
}
