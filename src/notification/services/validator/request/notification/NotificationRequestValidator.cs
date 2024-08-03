using Common.Helpers;
using Notification.Domain.Entites;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Validator;

namespace Notification.Service.Validator;

public class NotificationRequestValidator : INotificationRequestValidator
{
    private readonly IRateLimitRuleService _rateLimitRuleService;

    public NotificationRequestValidator(IRateLimitRuleService rateLimitRuleService)
    {
        _rateLimitRuleService = rateLimitRuleService;
    }

    public ValidatorResponse Validate(NotificationRequest request)
    {
        ValidatorResponse response = new();
        if (string.IsNullOrEmpty(request.Sender))
        {
            response.AddError($"Sender not valid with value [{request.Sender}]");
        }
        string senderKey = CacheKeyNameHelper.GetNotificationRuleKey(request.Sender);
        NotificationRule? ruleOnCache = _rateLimitRuleService.FindByKey(senderKey).Result;
        if (ruleOnCache == null)
        {
            response.AddError("Sender rule does not exists.");
        }
        if (string.IsNullOrEmpty(request.Recipient))
        {
            response.AddError($"Recipient is not valid with value [{request.Recipient}]");
        }
        if (string.IsNullOrEmpty(request.Message))
        {
            response.AddError("Request does not contains a message.");
        }

        return response;
    }
}
