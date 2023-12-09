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

Regex regex = MyRegex();

int sum = 0;
foreach (string line in lines)
{
    Match match = regex.Match(line);

    if (!match.Success)
    {
        throw new Exception("No numbers found in the input.");
    }

    string firstNumber = match.Groups[1].Value;
    string lastNumber = match.Groups[2].Value;
    if (lastNumber.Length == 0)
    {
        lastNumber = firstNumber;
    }

    string valueAsString = $"{firstNumber}{lastNumber}";
    if (valueAsString.Length != 2)
    {
        throw new Exception($"{valueAsString} is not correct");
    }

    int value = int.Parse(valueAsString);
    sum += value;
    Console.WriteLine($"value: {value}");
}

Console.WriteLine($"sum: {sum}");

partial class Program
{
    [GeneratedRegex(@"^[^\d]*(\d).*?(\d)?[^\d]*$")]
    private static partial Regex MyRegex();
}
