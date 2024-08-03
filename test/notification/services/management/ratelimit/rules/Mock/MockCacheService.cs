using Notification.Domain.Interfaces;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;

namespace Notification.Services.Management.RateLimit.Rules.Mock;

public class MockCacheService : ICacheService
{
    public object? FindResult { get; set; }

    public List<string> FunctionCalls { get; set; } = new List<string>();

    public string? ValueToBeCreated { get; set; }

    public string? DeleteKey { get; set; }

    public bool DeleteResult { get; set; } = false;
    public bool CreateResult { get; set; } = false;

    public long? ExpirationSetOnCreation { get; set; }

    public async Task<bool> Create(string key, string value, long? expireIn = null)
    {
        await Task.Yield();
        ExpirationSetOnCreation = expireIn;
        FunctionCalls.Add("Create");
        return CreateResult;
    }

    public async Task<string?> DecreaseValue(string key)
    {
        await Task.Yield();
        FunctionCalls.Add("DecreaseValue");
        throw new NotImplementedException();
    }

    public async Task<string?> Find(CacheRequest request)
    {
        await Task.Yield();
        FunctionCalls.Add("Find");
        return string.Empty;
    }

    public async Task<CacheResponse<TEntity>?> Find<TEntity>(CacheRequest request) where TEntity : class
    {
        await Task.Yield();
        FunctionCalls.Add("Find");
        return new CacheResponse<TEntity>(request.Key, (TEntity?)FindResult);
    }

    public async Task<bool> Remove(string key)
    {
        await Task.Yield();
        DeleteKey = key;
        FunctionCalls.Add("Delete");
        return DeleteResult;
    }
}
