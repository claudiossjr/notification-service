using System.Text.Json;
using Common.Helpers;
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

        CacheResponse<NotificationRule>? findNotificationRuleTask;
        string? findNotificationTokenBucketTask;
        string senderRuleKey = CacheKeyNameHelper.GetNotificationRuleKey(request.Sender);
        string recipientTokenBucketKey = CacheKeyNameHelper.GetNotificationTokenBucketKey(request.Sender, request.Recipient);
        try
        {
            findNotificationRuleTask = await _cacheService.Find<NotificationRule>(new(senderRuleKey));
            findNotificationTokenBucketTask = await _cacheService.IncreaseValue(new(recipientTokenBucketKey));
        }
        catch (CacheServerOfflineException)
        {
            // TODO: log error message
            return new(false, NotificationResponseCode.FailedDependency, $"Cannot get cached item.");
        }

        NotificationRule? senderRule;
        CacheResponse<NotificationRule>? ruleResponse = findNotificationRuleTask;
        if (ruleResponse == null)
        {
            return new(false, NotificationResponseCode.Notfound, $"There is no rule for the sender: {request.Sender}");
        }

        senderRule = ruleResponse.Value;
        if (senderRule == null)
        {
            return new(false, NotificationResponseCode.Notfound, $"There is no rule for the sender: {request.Sender}");
        }

        string? recipientTokenBucketResponse = findNotificationTokenBucketTask;
        try
        {
            if (string.IsNullOrEmpty(recipientTokenBucketResponse))
            {
                recipientTokenBucketResponse = "0";
                // Criar TokenBucket
                bool couldCreate = await _cacheService.Create(recipientTokenBucketKey, recipientTokenBucketResponse!, senderRule.ExpiresInMilliseconds);
                if (couldCreate == false)
                {
                    return await Notify(request);
                }
            }
        }
        catch (CacheServerOfflineException)
        {
            // TODO: log error message
            return new(false, NotificationResponseCode.FailedDependency, $"Cannot get cached item.");
        }

        if (senderRule.RateLimit - long.Parse(recipientTokenBucketResponse!) <= 0)
        {
            return new(false, NotificationResponseCode.TooManyRequest, "Number of calls exceed the limit.");
        }

        return new(true, NotificationResponseCode.Success, $"Notification to {request.Recipient} from {request.Sender} was sent!");
    }
}
