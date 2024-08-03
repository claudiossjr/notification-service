using Microsoft.Extensions.DependencyInjection;
using Notification.DI.Extensions;

namespace Notification.DI;

public static class ServicesResolversExtension
{
    public static IServiceCollection AddNotificationDIServices(this IServiceCollection services)
    {
        services.AddNotificationServiceResolver()
                .AddProviderServiceResolver()
                .AddRateLimitRulesServiceResolver();
        return services;
    }

}
