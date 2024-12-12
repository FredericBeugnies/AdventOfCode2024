var lines = File.ReadAllLines("input.txt");

Grid topo = new();
for (int r = 0; r < lines.Length; r++)
    for (int c = 0; c < lines[r].Length; c++)
        topo.Set(c, r, int.Parse(lines[r][c].ToString()));

// part 1

// reachable[x, y, z] = distinct 9-height positions that can be reached from (x, y), starting with the height z (i.e. mark a path z ... 9)
HashSet<Pos>[,,] reachable = new HashSet<Pos>[Problem.Size, Problem.Size, Problem.MaxHeight + 1];
for (int x = 0; x < Problem.Size; x++)
    for (int y = 0; y < Problem.Size; y++)
        for (int z = 0; z <= Problem.MaxHeight; z++)
            reachable[x, y, z] = new HashSet<Pos>();

// init with all 9-height positions
int zmax = Problem.MaxHeight;
for (int x = 0; x < Problem.Size; x++)
    for (int y = 0; y < Problem.Size; y++)
        if (topo.Get(x, y) == zmax)
            reachable[x, y, zmax].Add(new Pos(x, y));

for (int z = Problem.MaxHeight - 1; z >= 0; z--)
{
    for (int x = 0; x < Problem.Size; x++)
        for (int y = 0; y < Problem.Size; y++)
            if (topo.Get(x, y) == z)
            {
                if (!OutOfBounds(x - 1, y))
                    reachable[x, y, z].UnionWith(reachable[x - 1, y, z + 1]);
                if (!OutOfBounds(x + 1, y))
                    reachable[x, y, z].UnionWith(reachable[x + 1, y, z + 1]);
                if (!OutOfBounds(x, y - 1))
                    reachable[x, y, z].UnionWith(reachable[x, y - 1, z + 1]);
                if (!OutOfBounds(x, y + 1))
                    reachable[x, y, z].UnionWith(reachable[x, y + 1, z + 1]);
            }
}

int totalScore = 0;
for (int x = 0; x < Problem.Size; x++)
    for (int y = 0; y < Problem.Size; y++)
        totalScore += reachable[x, y, 0].Count();
Console.WriteLine(totalScore);

bool OutOfBounds(int x, int y)
{
    return x < 0 || y < 0 || x >= Problem.Size || y >= Problem.Size;
}

static class Problem
{
    public const int Size = 56;
    public const int MinHeight = 0;
    public const int MaxHeight = 9;
}

struct Pos
{
    public int x;
    public int y;

    public Pos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool OutOfBounds()
    {
        return x < 0 || y < 0 || x >= Problem.Size || y >= Problem.Size;
    }
}

class Grid
{
    public int[,] m = new int[Problem.Size, Problem.Size];

    public void Set(Pos pos, int val)
    {
        Set(pos.x, pos.y, val);
    }

    public void Set(int x, int y, int val)
    {
        m[x, y] = val;
    }

    public int? Get(int x, int y)
    {
        return OutOfBounds(x, y) ? null : m[x, y];
    }

    public bool OutOfBounds(int x, int y)
    {
        return x < 0 || y < 0 || x >= Problem.Size || y >= Problem.Size;
    }

    public void Print()
    {
        for (int y = 0; y < Problem.Size;y++)
        {
            for (int x = 0;x < Problem.Size;x++)
                Console.Write($"{m[x, y]} ");
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}