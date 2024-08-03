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
        NotificationRule? ruleOnCache = await FindByKey(ruleKey);
        if (ruleOnCache != null)
        {
            return false;
        }
        return await _cacheService.Create(ruleKey, JsonSerializer.Serialize(rule));
    }

    public async Task<bool> Delete(string key)
    {
        try
        {
            NotificationRule? ruleOnCache = await FindByKey(key);
            if (ruleOnCache == null)
            {
                return false;
            }
            return await _cacheService.Remove(key);
        }
        catch (Exception)
        {
            return false;
        }
    }


    public async Task<bool> Update(NotificationRule rule)
    {
        try
        {
            string ruleKey = CacheKeyNameHelper.GetNotificationRuleKey(rule.Sender);
            NotificationRule? ruleOnCache = await FindByKey(ruleKey);
            if (ruleOnCache == null)
            {
                return false;
            }
            await _cacheService.Remove(ruleKey);
            await _cacheService.Create(ruleKey, JsonSerializer.Serialize(rule));
            return true;
        }
        catch (Exception)
        {
            // Log Error
            return false;
        }
    }
}
