using System.Text.RegularExpressions;

var input = File.ReadAllLines("input24.txt");

// parse input
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

// compute part 1
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
                wire.Val = input1.Val && input2.Val;
            else if (wire.Operation == "OR")
                wire.Val = input1.Val || input2.Val;
            else
                wire.Val = input1.Val ^ input2.Val;
            wire.HasValue = true;
            valueChanged = true;
        }
    }
} while (valueChanged);

long res = 0;
foreach (var wire in wires.Values)
{
    if (wire.Name.StartsWith('z') && wire.HasValue && wire.Val)
    {
        int exp = int.Parse(wire.Name.Substring(1));
        res += PowerOf2(exp);
    }
}
Console.WriteLine($"Part 1: {res}");

// part 2
// this code does not flat-out give the puzzle answer, but tries to identify each node expected in a binary additioner
// and lists any discrepancy found along the way => allows to manually find the pairs of wires to swap

Console.WriteLine("Part 2:");

// uncomment these to test some pair swappings and check if that solves some discrepancies
//Swap("hmt", "z18");
//Swap("bfq", "z27");
//Swap("hkh", "z31");
//Swap("fjp", "bng");

Wire[] andA = new Wire[45];
Wire?[] andB = new Wire[45];
Wire[] xor = new Wire[45];
Wire?[] z = new Wire[45];
Wire?[] rem = new Wire[45];
foreach (var wire in wires.Values)
{
    if (!string.IsNullOrEmpty(wire.Operation))
    {
        if (wire.Operation == "AND" && wire.HasXYInput())
        {
            int n1 = int.Parse(wire.Input1.Substring(1));
            int n2 = int.Parse(wire.Input2.Substring(1));
            if (n1 == n2)
            {
                wire.Alias = $"AND_A_{n1}";
                andA[n1] = wire;
            }
        }
        if (wire.Operation == "XOR" && wire.HasXYInput())
        {
            int n1 = int.Parse(wire.Input1.Substring(1));
            int n2 = int.Parse(wire.Input2.Substring(1));
            if (n1 == n2)
            {
                wire.Alias = $"XOR_A_{n1}";
                xor[n1] = wire;
            }
        }
    }
}
rem[0] = andA[0];
for (int k = 1; k < 45; ++k)
{
    z[k] = FindWire("XOR", xor[k], rem[k - 1], $"Z_{k}");
    andB[k] = FindWire("AND", xor[k], rem[k - 1], $"AND_B_{k}");
    rem[k] = FindWire("OR", andB[k], andA[k], $"REM_{k}");

    if (z[k]?.HasValue == true && !z[k]?.Name.StartsWith('z') == true)
    {
        Console.WriteLine($"\t{z[k]?.Name} should be called {z[k]?.Alias}");
    }
}

long PowerOf2(int exponent)
{
    long res = 1;
    for (int i = 0; i < exponent; i++)
        res *= 2;
    return res;
}

void Swap(string name1, string name2)
{
    var w1 = wires[name1];
    var w2 = wires[name2];
    wires.Remove(name1);
    wires.Remove(name2);
    w1.Name = name2;
    w1.Alias = name2;
    w2.Name = name1;
    w2.Alias = name1;
    wires.Add(name2, w1);
    wires.Add(name1, w2);
}

Wire? FindWire(string operation, Wire? input1, Wire? input2, string alias)
{
    var wire = wires.Values.Where(x => x.Operation == operation && x.HasInput(input1) && x.HasInput(input2)).FirstOrDefault();
    if (wire == null)
    {
        wire = wires.Values.Where(x => x.Operation == operation && (x.HasInput(input1) || x.HasInput(input2))).FirstOrDefault();
        Console.WriteLine($"\t{alias}: Expected {input1?.Name} {operation} {input2?.Name}, but found {wire?.Input1} {wire?.Operation} {wire?.Input2}");
    }
    if (wire != null)
        wire.Alias = alias;
    return wire;
}

class Wire
{
    public string Name;
    public bool HasValue;
    public bool Val;
    public string Operation;
    public string Input1;
    public string Input2;
    public string Alias;

    public Wire(string name, bool value)
    {
        Name = Alias = name;
        HasValue = true;
        Val = value;
        Operation = "";
        Input1 = "";
        Input2 = "";
    }

    public Wire(string name, string operation, string input1, string input2) 
    {
        Name = Alias = name;
        HasValue = false;
        Operation = operation;
        Input1 = input1;
        Input2 = input2;
    }

    public bool HasInput(Wire? input)
    {
        if (input == null)
            return false;
        return Input1 == input.Name || Input2 == input.Name;
    }

    public bool HasXYInput()
    {
        return (Input1.StartsWith('x') && Input2.StartsWith('y')) || (Input1.StartsWith('y') && Input2.StartsWith('x'));
    }
}