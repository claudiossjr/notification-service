using Notification.Api.Controllers;

namespace Notification.Api.Routes.Extensions;

public static class RateLimiterRulesBuilderExtensions
{
    public static RouteGroupBuilder UseRateLimiterRulesRoutes(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/rules").WithTags("Manage Rate Limit Rules");

        group.MapGet("/{sender}", RateLimitRuleController.Get)
        .WithName("GetSenderConfig")
        .WithOpenApi();

        group.MapPost("/", RateLimitRuleController.Create)
        .WithName("CreateSenderConfig")
        .WithOpenApi();

        group.MapPut("/", RateLimitRuleController.Update)
        .WithName("UpdateSenderConfig")
        .WithOpenApi();

        group.MapDelete("/{sender}", RateLimitRuleController.Delete)
        .WithName("DeleteSenderConfig")
        .WithOpenApi();

        return group;
    }
}