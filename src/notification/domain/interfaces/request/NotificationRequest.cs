namespace Notification.Domain.Interfaces.Request;

public record NotificationRequest(string Sender, string Recipient, string Message);