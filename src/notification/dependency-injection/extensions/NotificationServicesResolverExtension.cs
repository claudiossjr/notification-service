using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Validator;
using Notification.Service;
using Notification.Service.Validator;

namespace Notification.DI.Extensions;

public static class NotificationServicesResolverExtension
{
    public static IServiceCollection AddNotificationServiceResolver(this IServiceCollection services)
    {
        services.AddScoped<INotificationRequestValidator, NotificationRequestValidator>();
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}