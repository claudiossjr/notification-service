namespace Notification.Domain.Interfaces.Response;

public record CacheResponse<TEntity>(string Key, TEntity? Value) where TEntity : class;
