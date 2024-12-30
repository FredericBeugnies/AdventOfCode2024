var input = File.ReadAllLines("input22.txt");

long res = 0;
foreach (var line in input)
{
    long secretNumber = long.Parse(line);
    for (int k = 0; k < 2000; ++k)
    {
        Mix(ref secretNumber, secretNumber * 64);
        Prune(ref secretNumber);

        Mix(ref secretNumber, secretNumber / 32);
        Prune(ref secretNumber);

        Mix(ref secretNumber, secretNumber * 2048);
        Prune(ref secretNumber);
    }
    res += secretNumber;
}

Console.WriteLine(res);

void Mix(ref long a, long b)
{
    a ^= b;
}

void Prune(ref long a)
{
    a %= 16777216;
}