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

RunProgram(program, registers, out var output);
WriteOutput(output);

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
        case 7: return default;
        default: return operand;
    }
}

void WriteOutput(List<int> output)
{
    Console.Write($"{output[0]}");
    for (int i = 1; i < output.Count; i++)
    {
        Console.Write($",{output[i]}");
    }
    Console.WriteLine();
}

enum Register
{
    A,
    B,
    C,
}