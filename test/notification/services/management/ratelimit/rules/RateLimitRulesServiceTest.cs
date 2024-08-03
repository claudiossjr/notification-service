using Common.Helpers;
using Notification.Domain.Entites;
using Notification.Services.Management.RateLimit.Rules.Mock;

namespace Notification.Services.Management.RateLimit.Rules.Test;

[Trait("UnitTest", "RateLimitRuleTest")]
public class RateLimitRulesServiceTest
{
    private readonly MockCacheService _mockCacheService;
    private readonly RateLimitRulesService _sut;

    public RateLimitRulesServiceTest()
    {
        _mockCacheService = new();
        _sut = new(_mockCacheService);
    }

    [Fact]
    public async void ShouldReturnFalseIfKeyAlreadyExistsOnCreation()
    {
        // Arrange
        NotificationRule rule = new()
        {
            Sender = "",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        _mockCacheService.FindResult = rule;

        // Act
        bool couldCreate = await _sut.Create(rule);

        // Assert
        Assert.False(couldCreate);
        Assert.Equal(["Find"], _mockCacheService.FunctionCalls);
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Find")));
        Assert.Null(_mockCacheService.ExpirationSetOnCreation);
    }

    [Fact]
    public async void ShouldCallFindBeforeCreatingData()
    {
        // Arrange
        NotificationRule rule = new()
        {
            Sender = "",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        _mockCacheService.CreateResult = true;

        // Act
        bool couldCreate = await _sut.Create(rule);

        // Assert
        Assert.Equal(["Find", "Create"], _mockCacheService.FunctionCalls, (a, b) => a.Equals(b, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Find")));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Create")));
        Assert.True(couldCreate);
        Assert.Null(_mockCacheService.ExpirationSetOnCreation);
    }

    [Fact]
    public async void ShouldReturnFalseIfKeyNotExistsOnDeletion()
    {
        // Arrange
        string searchingKey = "search_key";

        // Act
        bool couldCreate = await _sut.Delete(searchingKey);

        // Assert
        Assert.False(couldCreate);
        Assert.Equal(["Find"], _mockCacheService.FunctionCalls);
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Find")));
        Assert.Equal(0, _mockCacheService.FunctionCalls.Count(c => c.Equals("Delete")));
        Assert.Null(_mockCacheService.DeleteKey);
    }

    [Fact]
    public async void ShouldCallFindBeforeDeleting()
    {
        // Arrange
        NotificationRule rule = new()
        {
            Sender = "DeleteKey",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        _mockCacheService.FindResult = rule;
        _mockCacheService.DeleteResult = true;
        string deleteKey = "delete_key";

        // Act
        bool couldDelete = await _sut.Delete(deleteKey);

        // Assert
        Assert.True(couldDelete);
        Assert.Equal(["Find", "Delete"], _mockCacheService.FunctionCalls, (a, b) => a.Equals(b, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Find")));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Delete")));
        Assert.Equal(deleteKey, _mockCacheService.DeleteKey);
    }

    [Fact]
    public async void ShouldReturnFalseIfKeyNotExistsOnUpdate()
    {
        // Arrange
        NotificationRule rule = new()
        {
            Sender = "DeleteKey",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        _mockCacheService.FindResult = rule;
        _mockCacheService.DeleteResult = true;
        _mockCacheService.CreateResult = true;
        string deleteKey = CacheKeyNameHelper.GetNotificationRuleKey(rule.Sender);

        // Act
        bool couldDelete = await _sut.Update(rule);

        // Assert
        Assert.True(couldDelete);
        Assert.Equal(["Find", "Delete", "Create"], _mockCacheService.FunctionCalls, (a, b) => a.Equals(b, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Find")));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Delete")));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Create")));
        Assert.Equal(deleteKey, _mockCacheService.DeleteKey);
    }

    [Fact]
    public async void ShouldCallFindAndDeleteBeforeUpdating()
    {
        // Arrange
        NotificationRule rule = new()
        {
            Sender = "DeleteKey",
            RateLimit = 2,
            TimeSpanInSeconds = 10
        };
        _mockCacheService.FindResult = rule;
        _mockCacheService.DeleteResult = true;
        _mockCacheService.CreateResult = true;
        string deleteKey = CacheKeyNameHelper.GetNotificationRuleKey(rule.Sender);

        // Act
        bool couldDelete = await _sut.Update(rule);

        // Assert
        Assert.True(couldDelete);
        Assert.Equal(["Find", "Delete", "Create"], _mockCacheService.FunctionCalls, (a, b) => a.Equals(b, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Find")));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Delete")));
        Assert.Equal(1, _mockCacheService.FunctionCalls.Count(c => c.Equals("Create")));
        Assert.Equal(deleteKey, _mockCacheService.DeleteKey);
        Assert.Null(_mockCacheService.ExpirationSetOnCreation);
    }
}