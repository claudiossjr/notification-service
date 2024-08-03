namespace Notification.Domain.Entites;

public record NotificationRule(string Sender, long RateLimit, long ExpiresInMilliseconds);