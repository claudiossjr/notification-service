using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Interfaces;
using Notification.Services.Management.RateLimit.Rules;

namespace Notification.DI.Extensions;

public static class RateLimitRulesServicesResolverExtension
{
    public static IServiceCollection AddRateLimitRulesServiceResolver(this IServiceCollection services)
    {
        services.AddScoped<IRateLimitRuleService, RateLimitRulesService>();
        return services;
    }
}
