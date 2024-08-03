
using System.Text.RegularExpressions;
using Notification.Api.Routes.Extensions;

namespace Notification.Api.RateLimiterRules.Routes;

public static class NotificationRouteGroupExtension
{
    public static WebApplication UseNotificationRouteGroup(this WebApplication app)
    {
        app.MapGroup("/notification")
            .UseNotificationGatewayRoute()
            .UseRateLimiterRulesRoutes();
        return app;
    }
}