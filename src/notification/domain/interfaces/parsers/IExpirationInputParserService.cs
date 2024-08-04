namespace Notification.Domain.Interfaces.Parsers;

public interface IExpirationInputParserService
{
    Task<long> ParseInput(string input);
}