var input = File.ReadAllLines("input20.txt");

// parse input
Pos start = default;
char[,] grid = new char[Problem.Size, Problem.Size];
for (int y = 0; y < input.Length; y++)
    for (int x = 0; x < input[y].Length; x++)
    {
        grid[x, y] = input[y][x];
        if (grid[x, y] == 'S')
            start = new Pos(x, y);
    }

// part 1
var sp = ComputeShortestPaths(grid, start);
var cheats1 = ComputeCheats(grid, sp, 2);
Console.WriteLine($"Part 1: {cheats1.Count}");

// part 2
var cheats2 = ComputeCheats(grid, sp, 20);
Console.WriteLine($"Part 2: {cheats2.Count}");

List<Cheat> ComputeCheats(char[,] grid, CellState[,] shortestPaths, int cheatDuration)
{
    List<Cheat> cheats = [];
    for (int y = 0; y < Problem.Size; y++)
    {
        for (int x = 0; x < Problem.Size; x++)
        {
            if (grid[x, y] == '#')
                continue;
            if (!shortestPaths[x, y].IsReached)
                continue;

            for (int dy = -cheatDuration; dy <= cheatDuration; dy++)
            {
                for (int dx = -cheatDuration; dx <= cheatDuration; dx++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    if (Math.Abs(dx) + Math.Abs(dy) > cheatDuration)
                        continue;

                    Pos p1 = new Pos(x, y);
                    Pos p2 = new Pos(x + dx, y + dy);
                    if (OutOfBounds(p1) || OutOfBounds(p2))
                        continue;
                    if (grid[p1.x, p1.y] == '#' || grid[p2.x, p2.y] == '#')
                        continue;
                    int saving = shortestPaths[p1.x, p1.y].Distance - shortestPaths[p2.x, p2.y].Distance - Math.Abs(dx) - Math.Abs(dy);
                    if (saving >= 100)
                    {
                        cheats.Add(new Cheat
                        {
                            from = p1,
                            to = p2,
                            saving = saving,
                        });
                    }
                }
            }
        }
    }

    return cheats;
}

CellState[,] ComputeShortestPaths(char[,] grid, Pos start)
{
    // shortest path algorithm from day 16 without the orientation-related stuff.
    // => not the most suited algo for this problem, but still does the job
    CellState[,] state = new CellState[Problem.Size, Problem.Size];
    List<Pos> reached = [];
    for (int y = 0; y < Problem.Size; y++)
        for (int x = 0; x < Problem.Size; x++)
        {
            state[x, y] = new CellState();
            state[x, y].IsWall = grid[x, y] == '#';
        }
    state[start.x, start.y].IsReached = true;
    reached.Add(start);

    do
    {
        int minDistance = Int32.MaxValue;
        Pos closestNeighbour = default;

        // evaluate possible moves
        foreach (var pos in reached)
        {
            var r = state[pos.x, pos.y];
            if (r.AllNeighboursReached)
                continue;

            bool allNeighboursReached = true;
            foreach (var neighbour in GetNeighbours(pos))
            {
                if (OutOfBounds(neighbour))
                    continue;
                var neighbourState = state[neighbour.x, neighbour.y];
                if (neighbourState.IsWall || neighbourState.IsReached)
                    continue;
                allNeighboursReached = false;

                // compute distance from pos to neighbour
                int distance = r.Distance + 1;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNeighbour = neighbour;
                }
            }
            if (allNeighboursReached)
                state[pos.x, pos.y].AllNeighboursReached = true;
        }

        if (minDistance < Int32.MaxValue)
        {
            reached.Add(closestNeighbour);
            state[closestNeighbour.x, closestNeighbour.y].IsReached = true;
            state[closestNeighbour.x, closestNeighbour.y].Distance = minDistance;
        }
        else
        {
            break;
        }
    } while (true);

    return state;
}

bool OutOfBounds(Pos p)
{
return p.x < 0 || p.y < 0 || p.x >= Problem.Size || p.y >= Problem.Size;
}

List<Pos> GetNeighbours(Pos p)
{
return new List<Pos>
    {
        p + new Pos(0, 1),
        p + new Pos(0, -1),
        p + new Pos(1, 0),
        p + new Pos(-1, 0)
    };
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

    public static Pos operator +(Pos p1, Pos p2)
    {
        return new Pos
        {
            x = p1.x + p2.x,
            y = p1.y + p2.y
        };
    }
}

struct CellState
{
    public bool IsWall;
    public bool IsReached;
    public bool AllNeighboursReached;
    public int Distance;

    public CellState()
    {
        IsWall = false;
        IsReached = false;
        Distance = 0;
    }
}

struct Cheat
{
    public Pos from;
    public Pos to;
    public int saving;
}

static class Problem
{
    public const int Size = 141;
}