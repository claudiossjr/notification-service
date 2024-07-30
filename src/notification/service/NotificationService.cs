using System.Text.Json;
using System.Text.Json.Serialization;
using Notification.Domain.Entites;
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

    public async Task<NotificationResponse> Notify(NotificationRequest request)
    {
        ValidatorResponse validatorResponse = _notificationRequestValidator.Validate(request);
        if (validatorResponse.IsValid == false)
        {
            return new(validatorResponse.IsValid, NotificationResponseCode.BadRequest, validatorResponse.ErrorMessage);
        }

        string senderRuleKey = $"SenderRule:{request.Sender}";
        NotificationRule? senderRule;
        try
        {
            senderRule = await _cacheService.Find<NotificationRule>(new(senderRuleKey));
        }
        catch (CacheServerOfflineException)
        {
            // TODO: log error message
            return new(false, NotificationResponseCode.FailedDependency, $"Can not send notification message");
        }

        if (senderRule == null)
        {
            return new(false, NotificationResponseCode.Notfound, $"There is no rule for the sender: {request.Sender}");
        }

        string recipientTokenBucketKey = $"Bucket:{request.Recipient}";

        NotificationTokenBucket? recipientTokenBucket;
        try
        {
            recipientTokenBucket = await _cacheService.Find<NotificationTokenBucket>(new(recipientTokenBucketKey));
        }
        catch (CacheServerOfflineException)
        {
            // TODO: log error message
            return new(false, NotificationResponseCode.FailedDependency, $"Can not send notification message");
        }

        if (recipientTokenBucket == null)
        {
            // Criar TokenBucket
            recipientTokenBucket = new()
            {
                Key = recipientTokenBucketKey,
                TokensRemaining = senderRule.RateLimit
            };
            bool couldCreate = await _cacheService.Create(recipientTokenBucketKey, JsonSerializer.Serialize(recipientTokenBucket), senderRule.TimeSpanInSeconds);
            if (couldCreate == false)
            {
                return await Notify(request);
            }
        }

        if (recipientTokenBucket!.TokensRemaining <= 0)
        {
            return new(false, NotificationResponseCode.TooManyRequest, "Number of calls exceeded.");
        }

        return new(true, NotificationResponseCode.Success, string.Empty);
    }
}
