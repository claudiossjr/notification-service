using Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Notification.Api.Controllers.Requests;
using Notification.Domain.Entites;
using Notification.Domain.Exceptions.Parser;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Parsers;

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

    public static async Task<IResult> Create([FromBody] NotificationRuleRequest rule, [FromServices] IServiceProvider serviceProvider)
    {
        await Task.Yield();
        using IServiceScope scope = serviceProvider.CreateScope();
        IRateLimitRuleService rateLimitRuleService = scope.ServiceProvider.GetRequiredService<IRateLimitRuleService>();
        IExpirationInputParserService expirationInputParser = scope.ServiceProvider.GetRequiredService<IExpirationInputParserService>();
        try{
            long expiration = await expirationInputParser.ParseInputToMilliseconds(rule.ExpiredIn);
            bool create = await rateLimitRuleService.Create(new NotificationRule(rule.Sender, rule.RateLimit, expiration));
            if (create) return Results.Created();
        }
        catch(ExpirationExpressionNotValidException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        return Results.BadRequest();
    }

    public static async Task<IResult> Update([FromBody] NotificationRuleRequest rule, [FromServices] IServiceProvider serviceProvider)
    {
        await Task.Yield();
        using IServiceScope scope = serviceProvider.CreateScope();
        IRateLimitRuleService rateLimitRuleService = scope.ServiceProvider.GetRequiredService<IRateLimitRuleService>();
        IExpirationInputParserService expirationInputParser = scope.ServiceProvider.GetRequiredService<IExpirationInputParserService>();
        try{
            long expiration = await expirationInputParser.ParseInputToMilliseconds(rule.ExpiredIn);
            bool updated = await rateLimitRuleService.Update(new NotificationRule(rule.Sender, rule.RateLimit, expiration));
            if (updated) return Results.Ok();
        }
        catch(ExpirationExpressionNotValidException ex)
        {
            return Results.BadRequest(ex.Message);
        }
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