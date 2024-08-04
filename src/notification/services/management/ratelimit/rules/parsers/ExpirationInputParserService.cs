using System.Text.RegularExpressions;
using Notification.Domain.Exceptions.Parser;
using Notification.Domain.Interfaces.Parsers;

namespace Notification.Services.Management.RateLimit.Rules.Parsers;

public class ExpirationInputParserService : IExpirationInputParserService
{
    /// <summary>
    /// Parse expiration input to return the number in seconds
    /// ex.: 1d:20m:20s should be converted to (24*60*60)+(20*60)+20 = 87620 segundos
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    

    public async Task<long> ParseInputToSeconds(string input)
    {
        return await ParseInput(input);
    }

    public async Task<long> ParseInputToMilliseconds(string input)
    {
        return await ParseInput(input) * 1000;
    }

    private async Task<long> ParseInput(string input)
    {
        await Task.Yield();
        input = input?.ToLower() ?? "";
        (bool inputValid, List<string> splittedParts) = ValidateInput(input);
        if (inputValid == false) throw new ExpirationExpressionNotValidException(input);
        List<Task<long>> tasks = [];
        foreach (string timexpression in splittedParts)
        {
            tasks.Add(GetValue(timexpression));
        }
        long[] values = await Task.WhenAll(tasks);
        long parsedValue = values.Sum();
        return parsedValue;
    }

    private async Task<long> GetValue(string expression)
    {
        await Task.Yield();
        string numberPart = Regex.Match(expression, @"\d*").Value;
        string identifier = Regex.Match(expression, @"[dhms]").Value;
        long factor = long.Parse(numberPart);
        long offset = GetOffset(identifier);
        return factor * offset;
    }

    private long GetOffset(string identifier)
    {
        long offset =  identifier switch { 
            "d" => 24 * GetOffset("h"),
            "h" => 60 * GetOffset("m"),
            "m" => 60 * GetOffset("s"),
            _ => 1
        };
        return offset;
    }

    private (bool, List<string>) ValidateInput(string input)
    {
        List<string> splitParts = [];
        if (string.IsNullOrEmpty(input)) return (false, splitParts);
        if (Regex.IsMatch(input, @"^(\d{1,2}[dhms]:?)*$") == false) return (false, splitParts);
        splitParts = [.. input.Split(":", StringSplitOptions.RemoveEmptyEntries)];
        if (splitParts.Count <= 0) return (false, splitParts);
        bool hasDuplicated = ValidateDuplicateEntry(splitParts);
        if (hasDuplicated) return (false, splitParts);

        return (true, splitParts);
    }

    private bool ValidateDuplicateEntry(List<string> splitParts)
    {
        return splitParts.Select(p => Regex.Match(p, @"[dhms]").Value).Distinct().Count() != splitParts.Count();
    }

    public async Task<string> ConvertTimeToExpressionFromMilliseconds(long time)
    {
        long timeInSeconds = time/1000;
        return await ConvertTimeExpression(timeInSeconds);
    }

    public async Task<string> ConvertTimeToExpressionFromSeconds(long time)
    {
        return await ConvertTimeExpression(time);
    }

    private async Task<string> ConvertTimeExpression(long time)
    {
        await Task.Yield();
        List<string> timeTokens = ["d", "h", "m", "s"];
        List<string> timeExpressions = [];
        foreach (string timeTokenIdentifier in timeTokens)
        {   
            long offset = GetOffset(timeTokenIdentifier);
            long numberPart = time / offset;
            time %= offset;
            if (numberPart > 0)
            {
                string timeExpression = $"{numberPart}{timeTokenIdentifier}";
                timeExpressions.Add(timeExpression);
            }
        }
        return string.Join(":", timeExpressions);
    }
}
