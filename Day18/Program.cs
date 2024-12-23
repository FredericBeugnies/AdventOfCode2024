var input = File.ReadAllLines("input.txt");

// parse input
bool[,] grid = new bool[Problem.Size, Problem.Size];
for (int i = 0; i < 1024; ++i)
{
    var tokens = input[i].Split(',');
    int x = int.Parse(tokens[0]);
    int y = int.Parse(tokens[1]);
    grid[x, y] = true;
}

// part 1
var start = new Pos(0, 0);
var end = new Pos(Problem.Size - 1, Problem.Size - 1);
var sp = ComputeShortestPaths(grid, start);

Console.WriteLine($"Part 1: {sp[end.x, end.y].Distance}");

// part 2
int answer = 0;
for (int i = 1024; i < input.Length; ++i)
{
    var tokens = input[i].Split(',');
    int x = int.Parse(tokens[0]);
    int y = int.Parse(tokens[1]);
    grid[x, y] = true;

    var spi = ComputeShortestPaths(grid, start);

    if (!spi[end.x, end.y].IsReached)
    {
        answer = i;
        break;
    }
}

Console.WriteLine($"Part 2: {input[answer]}");

CellState[,] ComputeShortestPaths(bool[,] grid, Pos start)
{
    // shortest path algorithm from day 16 without the orientation-related stuff.
    // => not the most suited algo for this problem, but still does the job
    CellState[,] state = new CellState[Problem.Size, Problem.Size];
    List<Pos> reached = [];
    for (int y = 0; y < Problem.Size; y++)
        for (int x = 0; x < Problem.Size; x++)
        {
            state[x, y] = new CellState();
            state[x, y].IsWall = grid[x, y];
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

static class Problem
{
    public const int Size = 71;
}