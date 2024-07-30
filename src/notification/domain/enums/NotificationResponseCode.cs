namespace Notification.Domain.Enums;

public enum NotificationResponseCode
{
    Success = 200,
    Created = 201,
    BadRequest = 400,
    Notfound = 404,
    FailedDependency = 424,
    TooManyRequest = 429
}