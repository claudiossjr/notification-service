namespace Notification.Api.Controllers.Response;

public record NotificationRuleResponse(string Sender, long RateLimit, string ExpiredIn);