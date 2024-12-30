var input = File.ReadAllLines("input22.txt");

long sumLastSecretNumbers = 0;
Dictionary<int, int> bananasSold = []; // total bananas sold for a particular sequence hash
foreach (var line in input)
{
    long secretNumber = long.Parse(line);
    int[] price = new int[2001];
    int[] priceChange = new int[2001];
    HashSet<int> encounteredSequences = [];

    price[0] = (int)(secretNumber % 10);

    for (int k = 1; k <= 2000; ++k)
    {
        // compute next secret number
        Mix(ref secretNumber, secretNumber * 64);
        Prune(ref secretNumber);
        Mix(ref secretNumber, secretNumber / 32);
        Prune(ref secretNumber);
        Mix(ref secretNumber, secretNumber * 2048);
        Prune(ref secretNumber);

        price[k] = (int)(secretNumber % 10);
        priceChange[k] = price[k] - price[k - 1];
        if (k > 3)
        {
            int seqKey = ComputeSequenceKey(priceChange[k - 3], priceChange[k - 2], priceChange[k - 1], priceChange[k]);
            if (!encounteredSequences.Contains(seqKey))
            {
                encounteredSequences.Add(seqKey);

                bananasSold.TryAdd(seqKey, 0);
                bananasSold[seqKey] += price[k];
            }
        }
    }
    sumLastSecretNumbers += secretNumber;
}

// output part 1
Console.WriteLine($"Part 1: {sumLastSecretNumbers}");

// output part 2
int maxBananasSold = bananasSold.Values.Max();
Console.WriteLine($"Part 2: {maxBananasSold}");

void Mix(ref long a, long b)
{
    a ^= b;
}

void Prune(ref long a)
{
    a %= 16777216;
}

int ComputeSequenceKey(int a, int b, int c, int d)
{
    a += 9;
    b += 9;
    c += 9;
    d += 9;
    int t2 = 19 * 19;
    int t3 = t2 * 19;
    return a * t3 + b * t2 + c * 19 + d;
}
