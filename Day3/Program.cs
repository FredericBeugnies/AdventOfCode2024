using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");

long result = 0;
bool enabled = true;
long result2 = 0;
foreach (var line in lines)
{
    string regex = @"mul\((\d+),(\d+)\)|do\(\)|don't\(\)";
    var matches = Regex.Matches(line, regex);
    foreach (Match match in matches)
    {
        Console.WriteLine($"{match.Value} {match.Groups[1]} {match.Groups[2]}");
        if (match.Value == "do()")
            enabled = true;
        else if (match.Value == "don't()")
            enabled = false;
        else
        {
            int x = Int32.Parse(match.Groups[1].Value);
            int y = Int32.Parse(match.Groups[2].Value);
            result += x * y;
            if (enabled)
                result2 += x * y;
        }

    }
}
Console.WriteLine(result.ToString());
Console.WriteLine(result2.ToString());