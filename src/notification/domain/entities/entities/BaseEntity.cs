namespace Notification.Domain.Entites;

public class BaseEntity
{
    public string? Id { get; set; }
    public required int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}