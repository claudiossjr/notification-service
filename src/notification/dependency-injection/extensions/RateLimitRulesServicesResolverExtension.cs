using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Parsers;
using Notification.Services.Management.RateLimit.Rules;
using Notification.Services.Management.RateLimit.Rules.Parsers;

namespace Notification.DI.Extensions;

public static class RateLimitRulesServicesResolverExtension
{
    public static IServiceCollection AddRateLimitRulesServiceResolver(this IServiceCollection services)
    {
        services.AddScoped<IExpirationInputParserService, ExpirationInputParserService>();
        services.AddScoped<IRateLimitRuleService, RateLimitRulesService>();
        return services;
    }
}
