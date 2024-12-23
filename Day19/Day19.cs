var input = File.ReadAllLines("input19.txt");

var availableTowels = input[0].Split(',', StringSplitOptions.TrimEntries);
List<string> desiredDesigns = [];
for (int i = 2; i < input.Length; i++)
{
    desiredDesigns.Add(input[i]);
}

// part 1
int nbPossibleDesigns = 0;
foreach (var desiredDesign in desiredDesigns)
{
    if (IsPossible(desiredDesign, availableTowels))
    {
        ++nbPossibleDesigns;
    }
}
Console.WriteLine($"Part 1: {nbPossibleDesigns}");

bool IsPossible(string design, string[] availableTowels)
{
    // isPossible[i] = is it possible to make the [i ... end] part of 'design' using the available towels
    Dictionary<int, bool> isPossible = [];
    isPossible[design.Length] = true;

    for (int i = design.Length - 1; i >= 0; i--)
    {
        string subDesign = design.Substring(i);
        isPossible[i] = false;

        foreach (string t in availableTowels)
        {
            if (subDesign.StartsWith(t) && isPossible[i + t.Length])
            {
                isPossible[i] = true;
                break;
            }
        }
    }
    return isPossible[0];
}