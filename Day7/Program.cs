var lines = File.ReadAllLines("input.txt");

Console.WriteLine($"Part 1: {ComputePart1(lines)}");
Console.WriteLine($"Part 2: {ComputePart2(lines)}");

static long ComputePart1(string[] lines)
{
    long totalResult = 0;
    foreach (var line in lines)
    {
        Equation e = Equation.Parse(line);
        long originalTestValue = e.testValue;

        Queue<Equation> q = new Queue<Equation>();
        q.Enqueue(e);

        while (q.Count > 0)
        {
            e = q.Dequeue();

            if (e.numbers.Count == 2)
            {
                int a = e.numbers[0];
                int b = e.numbers[1];
                if (a + b == e.testValue || a * b == e.testValue)
                {
                    totalResult += originalTestValue;
                    q.Clear();
                    break;
                }
            }
            else
            {
                int a = e.numbers[0];
                long c = e.testValue;

                // generate "sub-equation" for * (if possible)
                if (c % a == 0)
                {
                    var e1 = new Equation
                    {
                        testValue = c / a,
                        numbers = e.numbers.Skip(1).ToList()
                    };
                    q.Enqueue(e1);
                }

                // generate "sub-equation" for +
                if (c > a)
                {
                    var e1 = new Equation
                    {
                        testValue = c - a,
                        numbers = e.numbers.Skip(1).ToList()
                    };
                    q.Enqueue(e1);
                }
            }
        }
    }

    return totalResult;
}

static long ComputePart2(string[] lines)
{
    long totalResult = 0;
    foreach (var line in lines)
    {
        Equation2 e = Equation2.Parse(line);
        var operatorCombos = GenerateOperatorCombos(e.numbers.Length - 1);

        foreach (var operatorCombo in operatorCombos)
        {
            if (e.testValue == Evaluate(e.numbers, operatorCombo))
            {
                totalResult += e.testValue;
                break;
            }
        }
    }

    return totalResult;
}

static long Evaluate(int[] numbers, List<Operator> operators)
{
    long result = numbers[0];

    for (int i = 1; i < numbers.Length; i++) 
    {
        switch(operators[i - 1]) 
        {
            case Operator.Add: result += numbers[i]; break;
            case Operator.Multiply: result *= numbers[i]; break;
            case Operator.Concatenate: result = long.Parse(result.ToString() + numbers[i].ToString()); break;
        }
    }

    return result;
}

static List<List<Operator>> GenerateOperatorCombos(int size)
{
    if (size == 1)
    {
        return
        [
            [Operator.Add],
            [Operator.Multiply],
            [Operator.Concatenate],
        ];
    }
    else
    {
        List<List<Operator>> result = [];
        var baseCombos = GenerateOperatorCombos(size - 1);
        foreach (var baseCombo in baseCombos) 
        {
            var l1 = baseCombo.ToList();
            l1.Add(Operator.Add);
            result.Add(l1);

            var l2 = baseCombo.ToList();
            l2.Add(Operator.Multiply);
            result.Add(l2);

            var l3 = baseCombo.ToList();
            l3.Add(Operator.Concatenate);
            result.Add(l3);
        }
        return result;
    }
}

class Equation
{
    public long testValue;
    public List<int> numbers = []; // stored in reverse order of input (i.e. right-to-left)

    public static Equation Parse(string line)
    {
        Equation e = new();

        var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        e.testValue = long.Parse(tokens[0].Remove(tokens[0].Length - 1));
        for (int i = tokens.Length - 1; i > 0; --i) // reverse order
        {
            e.numbers.Add(int.Parse(tokens[i]));
        }

        return e;
    }
}

class Equation2
{
    public long testValue;
    public int[] numbers = []; // stored left-to-right

    public static Equation2 Parse(string line)
    {
        Equation2 e = new();

        var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        e.testValue = long.Parse(tokens[0].Remove(tokens[0].Length - 1));
        e.numbers = new int[tokens.Length - 1];
        for (int i = 1; i < tokens.Length; ++i)
        {
            e.numbers[i-1] = int.Parse(tokens[i]);
        }

        return e;
    }
}

enum Operator
{
    Add,
    Multiply,
    Concatenate
};
