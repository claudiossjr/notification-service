namespace Notification.Domain.Interfaces.Parsers;

public interface IExpirationInputParserService
{
    Task<long> ParseInputToSeconds(string input);
    Task<long> ParseInputToMilliseconds(string input);

    Task<string> ConvertTimeToExpressionFromMilliseconds(long time);
    Task<string> ConvertTimeToExpressionFromSeconds(long time);
}