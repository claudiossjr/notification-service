using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Api.Controllers;

public static class NotificationController
{
    public static async Task<IResult> Notify([FromBody] NotificationRequest request, [FromServices] IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        INotificationService? notificationService = scope.ServiceProvider.GetService<INotificationService>();
        if (notificationService == null)
        {
            return Results.StatusCode(500);
        }
        NotificationResponse response = await notificationService.Notify(request);
        return Results.Json(response.Message, statusCode: (int)response.StatusCode);
    }
}