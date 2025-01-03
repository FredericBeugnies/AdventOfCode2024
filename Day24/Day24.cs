using System.Text.RegularExpressions;

var input = File.ReadAllLines("input24.txt");

Dictionary<string, Wire> wires = [];
int i = 0;
for (; i < input.Length; ++i)
{
    if (string.IsNullOrEmpty(input[i]))
        break;

    string regex = @"(.+): (0|1)";
    var matches = Regex.Matches(input[i], regex);

    string name = matches.First().Groups[1].Value;
    bool value = matches.First().Groups[2].Value == "1";
    wires.Add(name, new Wire(name, value));
}
for (++i; i < input.Length; ++i)
{
    var tokens = input[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    string name = tokens[4];
    string operation = tokens[1];
    string input1 = tokens[0];
    string input2 = tokens[2];
    wires.Add(name, new Wire(name, operation, input1, input2));
}

bool valueChanged;
do
{
    valueChanged = false;

    foreach (var wire in wires.Values)
    {
        if (wire.HasValue)
            continue;

        Wire input1 = wires[wire.Input1];
        Wire input2 = wires[wire.Input2];
        if (input1.HasValue && input2.HasValue)
        {
            if (wire.Operation == "AND")
                wire.Value = input1.Value && input2.Value;
            else if (wire.Operation == "OR")
                wire.Value = input1.Value || input2.Value;
            else
                wire.Value = input1.Value ^ input2.Value;
            wire.HasValue = true;
            valueChanged = true;
        }
    }
} while (valueChanged);

long res = 0;
foreach (var wire in wires.Values)
{
    if (wire.Name.StartsWith('z') && wire.HasValue && wire.Value)
    {
        int exp = int.Parse(wire.Name.Substring(1));
        res += PowerOf2(exp);
    }
}
Console.WriteLine($"Part 1: {res}");

long PowerOf2(int exponent)
{
    long res = 1;
    for (int i = 0; i < exponent; i++)
        res *= 2;
    return res;
}

class Wire
{
    public string Name;
    public bool HasValue;
    public bool Value;
    public string? Operation;
    public string? Input1;
    public string? Input2;

    public Wire(string name, bool value)
    {
        Name = name;
        HasValue = true;
        Value = value;
    }

    public Wire(string name, string operation, string input1, string input2) 
    {
        Name = name;
        HasValue = false;
        Operation = operation;
        Input1 = input1;
        Input2 = input2;
    }
}