var lines = File.ReadAllLines("input.txt");
var tokens = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
var stones = tokens.Select(x => long.Parse(x)).ToList();

StoneCollection stoneCollection = new();
foreach (var stone in stones)
    stoneCollection.AddStone(stone);

int nbBlinks = 75;
for (int k = 0; k < nbBlinks; k++)
{
    stoneCollection.Blink();
    Console.WriteLine($"After {k+1} blinks: {stoneCollection.TotalStoneCount()}");
}

class StoneCollection
{
    private Dictionary<long, long> stones = [];

    public void AddStone(long stoneNumber, long count = 1)
    {
        AddStone(stones, stoneNumber, count);
    }

    public void AddStone(Dictionary<long, long> stones, long stoneNumber, long count)
    {
        if (!stones.ContainsKey(stoneNumber))
            stones.Add(stoneNumber, 0);
        stones[stoneNumber] += count;
    }

    public void Blink()
    {
        Dictionary<long, long> stonesAfter = [];
        foreach (var stone in stones)
        {
            var stoneNumber = stone.Key;
            var stoneCount = stone.Value;
            if (stoneNumber == 0)
            {
                AddStone(stonesAfter, 1, stoneCount);
            }
            else
            {
                string s = stoneNumber.ToString();
                if (s.Length % 2 == 0)
                {
                    string s2 = s.Substring(0, s.Length / 2);
                    string s1 = s.Substring(s.Length / 2);
                    AddStone(stonesAfter, long.Parse(s1), stoneCount);
                    AddStone(stonesAfter, long.Parse(s2), stoneCount);
                }
                else
                {
                    AddStone(stonesAfter, stoneNumber * 2024, stoneCount);
                }
            }
        }
        stones = stonesAfter;
    }

    public long TotalStoneCount()
    {
        long res = 0;
        foreach (var stone in stones)
            res += stone.Value;
        return res;
    }
}