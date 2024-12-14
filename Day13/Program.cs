using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");

List<MachineData> machines = [];
for (int i = 0; i < lines.Length; i += 4)
    machines.Add(MachineData.Parse(lines[i], lines[i+1], lines[i+2]));

// part 1
long nbTokens = 0;
foreach (var m in machines)
{
    nbTokens += GetMinNbTokensToWin(m, 100);
}
Console.WriteLine(nbTokens);

// part 2
long nbTokens2 = 0;
foreach (var m in machines)
{
    m.px += 10000000000000;
    m.py += 10000000000000;
    nbTokens2 += GetMinNbTokensToWin(m, 10000000000000);
}
Console.WriteLine(nbTokens2);

long GetMinNbTokensToWin(MachineData machine, long maxPresses)
{
    long xa = machine.xa;
    long ya = machine.ya;
    long xb = machine.xb;
    long yb = machine.yb;
    long px = machine.px;
    long py = machine.py;

    // solve the system of 2 equations with 2 variables
    long num_a = yb * px - xb * py;
    long den_a = xa * yb - ya * xb;
    if (num_a % den_a == 0) // result should be integer
    {
        long a = num_a / den_a;
        long num_b = py - a * ya;
        if (num_b % yb == 0) // result should be integer
        {
            long b = num_b / yb;
            //Console.WriteLine($"Solution found: A = {a}, B = {b}");
            if (a >= 0 && b >= 0 && a <= maxPresses && b <= maxPresses)
            {
                return 3 * a + b;
            }
        }
    }

    return 0;
}

class MachineData
{
    public long xa;
    public long ya;
    public long xb;
    public long yb;
    public long px;
    public long py;

    static public MachineData Parse(string line1, string line2, string line3)
    {
        const string regex1 = @"Button [AB]: X\+(\d+), Y\+(\d+)";
        const string regex2 = @"Prize: X=(\d+), Y=(\d+)";

        MachineData res = new MachineData();

        var matches = Regex.Matches(line1, regex1);
        res.xa = long.Parse(matches[0].Groups[1].Value);
        res.ya = long.Parse(matches[0].Groups[2].Value);

        matches = Regex.Matches(line2, regex1);
        res.xb = long.Parse(matches[0].Groups[1].Value);
        res.yb = long.Parse(matches[0].Groups[2].Value);

        matches = Regex.Matches(line3, regex2);
        res.px = long.Parse(matches[0].Groups[1].Value);
        res.py = long.Parse(matches[0].Groups[2].Value);

        return res;
    }
}