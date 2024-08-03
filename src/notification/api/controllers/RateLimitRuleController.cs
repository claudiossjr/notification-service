using Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Notification.Domain.Entites;
using Notification.Domain.Interfaces;

namespace Notification.Api.Controllers;

public static class RateLimitRuleController
{
    public static async Task<IResult> Get(string sender, [FromServices] IServiceProvider serviceProvider)
    {
        await Task.Yield();
        using IServiceScope scope = serviceProvider.CreateScope();
        IRateLimitRuleService rateLimitRuleService = scope.ServiceProvider.GetRequiredService<IRateLimitRuleService>();
        string searchKey = CacheKeyNameHelper.GetNotificationRuleKey(sender);
        NotificationRule? rule = await rateLimitRuleService.FindByKey(searchKey);
        if (rule == null) return Results.NotFound();
        return Results.Json(rule, statusCode: 200);
    }

    public static async Task<IResult> Create([FromBody] NotificationRule rule, [FromServices] IServiceProvider serviceProvider)
    {
        await Task.Yield();
        using IServiceScope scope = serviceProvider.CreateScope();
        IRateLimitRuleService rateLimitRuleService = scope.ServiceProvider.GetRequiredService<IRateLimitRuleService>();
        bool create = await rateLimitRuleService.Create(rule);
        if (create) return Results.Created();
        return Results.BadRequest();
    }

    public static async Task<IResult> Update([FromBody] NotificationRule rule, [FromServices] IServiceProvider serviceProvider)
    {
        await Task.Yield();
        using IServiceScope scope = serviceProvider.CreateScope();
        IRateLimitRuleService rateLimitRuleService = scope.ServiceProvider.GetRequiredService<IRateLimitRuleService>();
        bool updated = await rateLimitRuleService.Update(rule);
        if (updated) return Results.Ok();
        return Results.BadRequest();
    }

    public static async Task<IResult> Delete(string sender, [FromServices] IServiceProvider serviceProvider)
    {
        await Task.Yield();
        using IServiceScope scope = serviceProvider.CreateScope();
        IRateLimitRuleService rateLimitRuleService = scope.ServiceProvider.GetRequiredService<IRateLimitRuleService>();
        string searchKey = CacheKeyNameHelper.GetNotificationRuleKey(sender);
        bool deleted = await rateLimitRuleService.Delete(searchKey);
        if (deleted) return Results.NoContent();
        return Results.BadRequest();
    }

}