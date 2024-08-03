namespace Notification.Api.Routes.Extensions;

public static class RateLimiterRulesBuilderExtensions
{
    public static RouteGroupBuilder UseRateLimiterRulesRoutes(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/rules").WithTags("Manage Sender Rules");

        group.MapGet("/{sender}", (string sender) =>
        {
            return $"Searching by Sender {sender}";
        })
        .WithName("GetSenderConfig")
        .WithOpenApi();

        group.MapPost("/", () =>
        {
            return $"Created";
        })
        .WithName("CreateSenderConfig")
        .WithOpenApi();

        group.MapPut("/", () =>
        {
            return "Updating";
        })
        .WithName("UpdateSenderConfig")
        .WithOpenApi();

        group.MapDelete("/{key}", (string key) =>
        {
            return $"Deleting key {key}";
        })
        .WithName("DeleteSenderConfig")
        .WithOpenApi();

        return group;
    }
}