using System.Text.RegularExpressions;

bool isWatch = Environment.GetEnvironmentVariable("DOTNET_WATCH") == "1";
if (isWatch)
{
    Console.Clear();
}

CancellationTokenSource cts = new();
Console.CancelKeyPress += (_, e) =>
{
    Console.WriteLine("Canceling...");
    cts.Cancel();
    e.Cancel = true;
};

Console.WriteLine($"""args: ['{String.Join("', '", args)}']""");
string[] lines = await File.ReadAllLinesAsync(args[0], cts.Token);

Regex regex = GetFirstAndLastNumber();

int sum = 0;
foreach (string line in lines)
{
    Console.WriteLine($"line: {line}");

    Match match = regex.Match(line);

    if (!match.Success)
    {
        throw new Exception($"No numbers found in the input, {line}.");
    }

    string firstNumber = match.Groups[1].Value;
    string lastNumber = match.Groups[2].Value;

    int first = GetNumber(firstNumber) ?? throw new InvalidOperationException($"First can't be null. {line}");
    int last = GetNumber(lastNumber) ?? first;
    int value = 10 * first + last;
    sum += value;
    Console.WriteLine($"value: {value}");
}

Console.WriteLine($"sum: {sum}");

partial class Program
{
    private const string NumberRegex = @"(?:\d|zero|one|two|three|four|five|six|seven|eight|nine)";

    private const string NegativeLookAhead = $@"(?:(?!{NumberRegex}).)*";

    private const string NegativeLookBehind = $@"(?:.(?<!{NumberRegex}))*";

    [GeneratedRegex($@"^{NegativeLookAhead}({NumberRegex}).*?({NumberRegex})?{NegativeLookBehind}$")]
    private static partial Regex GetFirstAndLastNumber();

    public static int? GetNumber(string intAsString)
    {
        int? numberHelper = GetNumberHelper(intAsString);
        if (numberHelper is > 9 or < 0)
        {
            throw new InvalidOperationException($"numberHelper {numberHelper} needs to be between 0-9");
        }

        return numberHelper;
    }

    private static int? GetNumberHelper(string intAsString)
    {
        return int.TryParse(intAsString, out int attempt) ? attempt : ConvertNumberWordToInt(intAsString);
    }

    public static int? ConvertNumberWordToInt(string word)
    {
        return word.ToLower() switch
        {
            "zero" => 0,
            "one" => 1,
            "two" => 2,
            "three" => 3,
            "four" => 4,
            "five" => 5,
            "six" => 6,
            "seven" => 7,
            "eight" => 8,
            "nine" => 9,
            _ => null
        };
    }
}
