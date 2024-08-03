using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Interfaces;
using Notification.Provider.Cache.Local;

namespace Notification.DI.Extensions;

public static class ProviderServicesResolverExtension
{
    public static IServiceCollection AddProviderServiceResolver(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, LocalMemoryCacheService>();
        return services;
    }
}