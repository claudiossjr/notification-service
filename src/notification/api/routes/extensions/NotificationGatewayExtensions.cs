using Notification.Api.Controllers;

namespace Notification.Api.Routes.Extensions;

public static class NotificationGatewayExtensions
{
    public static RouteGroupBuilder UseNotificationGatewayRoute(this RouteGroupBuilder builder)
    {
        builder.MapPost("/", NotificationController.Notify)
        .WithTags("Execute Notification")
        .WithOpenApi();
        return builder;
    }
}