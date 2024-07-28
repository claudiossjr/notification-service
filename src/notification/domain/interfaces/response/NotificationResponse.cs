namespace Notification.Domain.Interfaces.Response;

public record NotificationResponse(bool HasSucceed, int StatusCode, string? ErroMessage);