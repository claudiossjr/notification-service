using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Notification.Domain.Entites;
using Notification.Domain.Interfaces.Request;
using Notification.Domain.Interfaces.Response;
using Notification.Provider.Cache.Local;

namespace Notification.Provider.Cache.Test;

[Trait("UnitTest", "LocalMemoryCache")]
public class LocalMemoryCacheServiceTest
{
    private readonly LocalMemoryCacheService _sut;

    public LocalMemoryCacheServiceTest()
    {
        MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        _sut = new(memoryCache);
    }

    [Fact]
    public async void ShouldReturnNullIfKeyDoesNotExists()
    {
        // Arrange
        string searchKey = "key";

        // Act
        CacheResponse<NotificationTokenBucket>? response = await _sut.Find<NotificationTokenBucket>(new CacheRequest(searchKey));

        Assert.NotNull(response);
        Assert.Equal(searchKey, response.Key);
        Assert.Null(response.Value);
    }

    [Fact]
    public async void ShouldReturnTrueWhenCreateACache()
    {
        // Arrange

        // Act
        bool couldCreate = await _sut.Create("key", "value");

        // Assert
        Assert.True(couldCreate);
    }

    [Fact]
    public async void ShouldReturnTheCacheResultIfKeyExists()
    {
        // Arrange
        NotificationRule rule = new("Sender", 2, 60);
        await _sut.Create("key", JsonSerializer.Serialize(rule));

        // Act
        CacheResponse<NotificationRule>? response = await _sut.Find<NotificationRule>(new CacheRequest("key"));

        Assert.NotNull(response);
        Assert.Equal(rule.Sender, response.Value!.Sender);
        Assert.Equal(rule.RateLimit, response.Value!.RateLimit);
        Assert.Equal(rule.ExpiresInMilliseconds, response.Value!.ExpiresInMilliseconds);

    }


    [Fact]
    public async void ShouldReturnNullWhenCacheEntryExpires()
    {
        // Arrange
        string searchKey = "key";
        long expireInSeconds = 20;
        NotificationRule rule = new("Sender", expireInSeconds, expireInSeconds);
        await _sut.Create(searchKey, JsonSerializer.Serialize(rule), expireInSeconds);

        // Act
        CacheResponse<NotificationRule>? response = await _sut.Find<NotificationRule>(new CacheRequest(searchKey));

        // Assert
        Assert.NotNull(response);
        Assert.Equal(rule.Sender, response.Value!.Sender);
        Assert.Equal(rule.RateLimit, response.Value!.RateLimit);
        Assert.Equal(rule.ExpiresInMilliseconds, response.Value!.ExpiresInMilliseconds);
        await Task.Delay(TimeSpan.FromMilliseconds(rule.ExpiresInMilliseconds));
        response = await _sut.Find<NotificationRule>(new CacheRequest(searchKey));
        Assert.NotNull(response);
        Assert.Equal(searchKey, response.Key);
        Assert.Null(response.Value);
    }

    [Fact]
    public async void ShouldDecrementValueIfKeyExists()
    {
        string searchKey = "test";
        long expirationTime = 20;
        await _sut.Create("test", "2", expirationTime);

        string? response = await _sut.Find(new CacheRequest(searchKey));
        string? decreasedResponse = await _sut.DecreaseValue(searchKey);
        await Task.Delay(TimeSpan.FromMilliseconds(expirationTime));
        string? expiredResponse = await _sut.Find(new CacheRequest(searchKey));

        Assert.NotNull(response);
        Assert.Equal("2", response);
        Assert.NotNull(decreasedResponse);
        Assert.Equal("1", decreasedResponse);
        Assert.Null(expiredResponse);
    }
}