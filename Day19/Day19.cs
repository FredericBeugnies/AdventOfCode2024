var input = File.ReadAllLines("input19.txt");

var availableTowels = input[0].Split(',', StringSplitOptions.TrimEntries);
List<string> desiredDesigns = [];
for (int i = 2; i < input.Length; i++)
{
    desiredDesigns.Add(input[i]);
}

// part 1 & 2 combined
int nbDesignsPossible = 0;
long totalNbArrangements = 0;
foreach (var desiredDesign in desiredDesigns)
{
    long nbArrangements = ComputeNbArrangements(desiredDesign, availableTowels);
    if (nbArrangements > 0)
        ++nbDesignsPossible;
    totalNbArrangements += nbArrangements;
}
Console.WriteLine($"Part 1: {nbDesignsPossible}");
Console.WriteLine($"Part 2: {totalNbArrangements}");

long ComputeNbArrangements(string design, string[] availableTowels)
{
    // nbWays[i] = number of ways it is possible make the [i ... end] part of 'design' using the available towels
    Dictionary<int, long> nbArrangements = [];
    nbArrangements[design.Length] = 1;

    for (int i = design.Length - 1; i >= 0; i--)
    {
        string subDesign = design.Substring(i);
        nbArrangements[i] = 0;

        foreach (string t in availableTowels)
        {
            if (subDesign.StartsWith(t))
            {
                nbArrangements[i] += nbArrangements[i + t.Length];
            }
        }
    }
    return nbArrangements[0];
}