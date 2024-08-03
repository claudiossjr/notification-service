using System.Text.Json;
using Common.Helpers;
using Notification.Domain.Entites;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Services.Management.RateLimit.Rules;

public class RateLimitRulesService : IRateLimitRuleService
{
    private readonly ICacheService _cacheService;

    public RateLimitRulesService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
    public async Task<NotificationRule?> FindByKey(string key)
    {
        CacheResponse<NotificationRule>? cacheResponse = await _cacheService.Find<NotificationRule>(new CacheRequest(key));
        return cacheResponse?.Value;
    }

    public async Task<bool> Create(NotificationRule rule)
    {
        string ruleKey = CacheKeyNameHelper.GetNotificationRuleKey(rule.Sender);
        NotificationRule? findOnCache = await FindByKey(ruleKey);
        if (findOnCache != null)
        {
            return false;
        }
        return await _cacheService.Create(ruleKey, JsonSerializer.Serialize(rule), rule.TimeSpanInSeconds);
    }

    public async Task<bool> Delete(string key)
    {
        return await _cacheService.Remove(key);
    }


    public async Task<bool> Update(NotificationRule rule)
    {
        try
        {
            string ruleKey = CacheKeyNameHelper.GetNotificationRuleKey(rule.Sender);
            NotificationRule? findOnCache = await FindByKey(ruleKey);
            if (findOnCache == null)
            {
                return false;
            }
            await Delete(ruleKey);
            await Create(rule);
            return true;
        }
        catch (Exception)
        {
            // Log Error
            return false;
        }
    }
}
