var lines = File.ReadAllLines("input.txt");

// parse input
int[] registers = new int[3];
List<int> program = [];

for (int i = 0; i < 3; i++)
{
    var tokensR = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    registers[i] = int.Parse(tokensR[2]);
}

var tokensP = lines[4].Replace("Program: ", "").Split(',', StringSplitOptions.RemoveEmptyEntries);
foreach (var token in tokensP)
    program.Add(int.Parse(token));

// part 1
RunProgram(program, registers, out var output);
Console.WriteLine($"Part 1: {CollateOutput(output)}");

// part 2
List<long>[] candidates = new List<long>[program.Count + 1];
candidates[program.Count] = [0]; // A == 0 at the end of the last iteration

for (int i = program.Count - 1; i >= 0; i--)
{
    // compute candidates for value of 'A' at end of iteration 'i'
    // based on next iteration outcome
    candidates[i] = [];
    foreach (var c in candidates[i + 1])
    {
        for (int modulo = 0; modulo < 8; ++modulo)
        {
            long a = c * 8 + modulo; // candidate for value 'A' at iteration i

            // check if subsequent 'B' value is consistent with program
            long bb = (int)(a % 8) ^ 5;
            long b = (bb ^ 6) ^ (long)(a / Math.Pow(2, bb));
            if (b % 8 == program[i])
                candidates[i].Add(a);
        }
    }
}

// turns out the candidates list is already sorted
Console.WriteLine($"Part 2: {candidates[0][0]}");

void RunProgram(List<int> program, int[] registers, out List<int> output)
{
    output = [];
    int instPtr = 0;

    while (instPtr < program.Count)
    {
        int operand = program[instPtr + 1];
        int nextInstPtr = instPtr + 2;
        int den;

        switch (program[instPtr])
        {
            case 0:
                den = (int)Math.Pow(2, GetComboOperandValue(operand, registers));
                registers[0] = registers[0] / den;
                break;
            case 1:
                registers[1] = registers[1] ^ operand;
                break;
            case 2:
                registers[1] = GetComboOperandValue(operand, registers) % 8;
                break;
            case 3:
                nextInstPtr = (registers[0] != 0) ? operand : nextInstPtr;
                break;
            case 4:
                registers[1] = registers[1] ^ registers[2];
                break;
            case 5:
                output.Add(GetComboOperandValue(operand, registers) % 8);
                break;
            case 6:
                den = (int)Math.Pow(2, GetComboOperandValue(operand, registers));
                registers[1] = registers[0] / den;
                break;
            case 7:
                den = (int)Math.Pow(2, GetComboOperandValue(operand, registers));
                registers[2] = registers[0] / den;
                break;
        }

        instPtr = nextInstPtr;
    }
}

int GetComboOperandValue(int operand, int[] registers)
{
    switch (operand)
    {
        case 4: return registers[0];
        case 5: return registers[1];
        case 6: return registers[2];
        case 7: return default; // should never happen
        default: return operand;
    }
}

string CollateOutput(List<int> output)
{
    return string.Join(',', output);
}
