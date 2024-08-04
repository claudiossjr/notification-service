using Notification.Domain.Exceptions.Parser;
using Notification.Services.Management.RateLimit.Rules.Parsers;

namespace Notification.Services.Management.RateLimit.Rules.Test;


[Trait("UnitTest","ExpirationParserInput")]
public class ExpirationInputParserServiceTest
{

    private readonly ExpirationInputParserService _sut;

    public ExpirationInputParserServiceTest()
    {
        _sut = new();
    }

    [Theory]
    [InlineData([""])]
    [InlineData([null])]
    public async Task ShouldThrowExceptionIfStringEmptyOrNullAsync(string input)
    {
        await Assert.ThrowsAsync<ExpirationExpressionNotValidException>(async () => await _sut.ParseInputToSeconds(input));
    }

    [Theory]
    [InlineData([":"])]
    [InlineData(["1"])]
    [InlineData(["s"])]
    [InlineData(["1x"])]
    [InlineData(["1h:1x"])]
    [InlineData(["1H:1X"])]
    [InlineData(["1H:1H"])]
    public async Task ShouldThrowExceptionHasNotTimeIdenficationOnExpressionAsync(string input)
    {
        await Assert.ThrowsAsync<ExpirationExpressionNotValidException>(async () => await _sut.ParseInputToSeconds(input));
    }

    [Theory]
    [InlineData(["1s", 1])]
    [InlineData(["1m", 60])]
    [InlineData(["1h", 3600])]
    [InlineData(["1d", 24*60*60])]
    [InlineData(["1m:1s", 61])]
    public async void ShouldReturnExpirationTimeInSecondsFromValidExpressionInSeconds(string input, long expectedValue)
    {

        // Act 
        long expiredIn = await _sut.ParseInputToSeconds(input);

        // Assert
         Assert.Equal(expectedValue, expiredIn);

    }

    [Theory]
    [InlineData(["1s", 1 * 1000])]
    [InlineData(["1m", 60 * 1000])]
    [InlineData(["1h", 3600 * 1000])]
    [InlineData(["1d", 24*60*60 * 1000])]
    [InlineData(["1m:1s", 61 * 1000])]
    public async void ShouldReturnExpirationTimeInSecondsFromValidExpressionInMilliseconds(string input, long expectedValue)
    {

        // Act 
        long expiredIn = await _sut.ParseInputToMilliseconds(input);

        // Assert
         Assert.Equal(expectedValue, expiredIn);

    }
}