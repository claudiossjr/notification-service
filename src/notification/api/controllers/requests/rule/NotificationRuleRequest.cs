namespace Notification.Api.Controllers.Requests;

public record NotificationRuleRequest(string Sender, int RateLimit, string ExpiredIn);