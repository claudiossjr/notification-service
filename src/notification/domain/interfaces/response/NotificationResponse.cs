using Notification.Domain.Enums;

namespace Notification.Domain.Interfaces.Response;

public record NotificationResponse(bool HasSucceed, NotificationResponseCode StatusCode, string? ErroMessage);