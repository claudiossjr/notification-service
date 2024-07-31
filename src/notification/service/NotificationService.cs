using System.Text.Json;
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

        Task<CacheResponse<NotificationRule>?> findNotificationRuleTask;
        Task<CacheResponse<NotificationTokenBucket>?> findNotificationTokenBucketTask;
        string senderRuleKey = $"SenderRule:{request.Sender}";
        string recipientTokenBucketKey = $"Bucket:{request.Sender}:{request.Recipient}";
        try
        {
            findNotificationRuleTask = _cacheService.Find<NotificationRule>(new(senderRuleKey));
            findNotificationTokenBucketTask = _cacheService.Find<NotificationTokenBucket>(new(recipientTokenBucketKey));
        }
        catch (CacheServerOfflineException)
        {
            // TODO: log error message
            return new(false, NotificationResponseCode.FailedDependency, $"Cannot get cached item.");
        }

        NotificationRule? senderRule;
        CacheResponse<NotificationRule>? ruleResponse = await findNotificationRuleTask;
        if (ruleResponse == null)
        {
            return new(false, NotificationResponseCode.Notfound, $"There is no rule for the sender: {request.Sender}");
        }

        senderRule = ruleResponse.Value;
        if (senderRule == null)
        {
            return new(false, NotificationResponseCode.Notfound, $"There is no rule for the sender: {request.Sender}");
        }

        NotificationTokenBucket? recipientTokenBucket;
        try
        {
            CacheResponse<NotificationTokenBucket>? recipientTokenBucketResponse = await findNotificationTokenBucketTask;
            if (recipientTokenBucketResponse == null)
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
            else
            {
                recipientTokenBucket = recipientTokenBucketResponse.Value;
            }
        }
        catch (CacheServerOfflineException)
        {
            // TODO: log error message
            return new(false, NotificationResponseCode.FailedDependency, $"Cannot get cached item.");
        }

        if (recipientTokenBucket!.TokensRemaining <= 0)
        {
            return new(false, NotificationResponseCode.TooManyRequest, "Number of calls exceed the limit.");
        }

        return new(true, NotificationResponseCode.Success, string.Empty);
    }
}
