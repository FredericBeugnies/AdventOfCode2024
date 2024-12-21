var lines = File.ReadAllLines("input.txt");

Pos start = default;
Pos end = default;
char[,] grid = new char[Problem.Size, Problem.Size];
for (int y = 0; y < lines.Length; y++)
    for (int x = 0; x < lines[y].Length; x++)
    {
        grid[x, y] = lines[y][x];
        if (grid[x, y] == 'S')
            start = new Pos(x, y);
        else if (grid[x, y] == 'E')
            end = new Pos(x, y);
    }

var originalGrid = grid.Clone() as char[,]; // keep original for diaply purposes at the end
CloseDeadEnds(grid);

// part 1
var shortestPathsFromStart = ComputeShortestPaths(grid, start, Orientation.Right);
int targetDistance = shortestPathsFromStart[end.x, end.y].Distance;

// part 2
var shortestPathsFromEnd = ComputeShortestPaths(grid, end, Orientation.Down);

int nbBigOs = 0;
bool[,] bigOs = new bool[Problem.Size, Problem.Size];
for (int y = 0; y < lines.Length; y++)
    for (int x = 0; x < lines[y].Length; x++)
    {
        if (grid[x, y] == '#')
            continue;
        var pathS = shortestPathsFromStart[x, y];
        var pathE = shortestPathsFromEnd[x, y];
        int distance = pathS.Distance + pathE.Distance + NbTurns(pathS.ReachedOrientation, GetOpposite(pathE.ReachedOrientation)) * 1000;
        if (distance == targetDistance) // should never be <
        {
            bigOs[x, y] = true;
            ++nbBigOs;
        }
    }

// output results
DisplayGrid2(originalGrid, bigOs);
Console.WriteLine($"Part 1: {targetDistance}");
Console.WriteLine($"Part 2: {nbBigOs}");

CellState[,] ComputeShortestPaths(char[,] grid, Pos start, Orientation o)
{
    CellState[,] state = new CellState[Problem.Size, Problem.Size];
    List<Pos> reached = [];
    for (int y = 0; y < lines.Length; y++)
        for (int x = 0; x < lines[y].Length; x++)
        {
            state[x, y] = new CellState();
            if (grid[x, y] == '#') state[x, y].IsWall = true;
        }
    state[start.x, start.y].IsReached = true;
    state[start.x, start.y].ReachedOrientation = o;
    reached.Add(start);

    do
    {
        int minDistance = Int32.MaxValue;
        OrientedPos closestNeighbour = default;

        // evaluate possible moves
        foreach (var pos in reached)
        {
            var r = state[pos.x, pos.y];
            if (r.AllNeighboursReached)
                continue;

            bool allNeighboursReached = true;
            foreach (var neighbour in GetNeighbours(pos))
            {
                var neighbourState = state[neighbour.p.x, neighbour.p.y];
                if (neighbourState.IsWall || neighbourState.IsReached)
                    continue;
                allNeighboursReached = false;

                // compute distance from pos to neighbour
                int distance = r.Distance + 1 + NbTurns(r.ReachedOrientation, neighbour.o) * 1000;
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
            reached.Add(closestNeighbour.p);
            state[closestNeighbour.p.x, closestNeighbour.p.y].IsReached = true;
            state[closestNeighbour.p.x, closestNeighbour.p.y].Distance = minDistance;
            state[closestNeighbour.p.x, closestNeighbour.p.y].ReachedOrientation = closestNeighbour.o;
        }
        else
        {
            break;
        }
    } while (true);

    return state;
}

void CloseDeadEnds(char[,] grid)
{
    bool deadEndFound;
    do
    {
        deadEndFound = false;
        for (int y = 1; y < grid.GetLength(0) - 1; y++)
        {
            for (int x = 1; x < grid.GetLength(1) - 1; x++)
            {
                if (grid[x, y] != '.')
                    continue;

                int nbWalls = 0;
                if (grid[x - 1, y] == '#') ++nbWalls;
                if (grid[x + 1, y] == '#') ++nbWalls;
                if (grid[x, y - 1] == '#') ++nbWalls;
                if (grid[x, y + 1] == '#') ++nbWalls;
                if (nbWalls == 3)
                {
                    deadEndFound = true;
                    grid[x, y] = '#';
                }
            }
        }
    } while (deadEndFound);
}

void DisplayGrid2(char[,] grid, bool[,] bigOs)
{
    for (int y = 0; y < grid.GetLength(0); y++)
    {
        for (int x = 0; x < grid.GetLength(1); x++)
            if (bigOs[x, y])
                Console.Write('O');
            else
                Console.Write(grid[x, y]);
        Console.WriteLine();
    }
    Console.WriteLine();
}

List<OrientedPos> GetNeighbours(Pos p)
{
    return new List<OrientedPos>
    {
        new OrientedPos(p + new Pos(0, 1), Orientation.Down),
        new OrientedPos(p + new Pos(0, -1), Orientation.Up),
        new OrientedPos(p + new Pos(1, 0), Orientation.Right),
        new OrientedPos(p + new Pos(-1, 0), Orientation.Left)
    };
}

Orientation GetOpposite(Orientation o)
{
    switch (o)
    {
        case Orientation.Up: return Orientation.Down;
        case Orientation.Down: return Orientation.Up;
        case Orientation.Left: return Orientation.Right;
        case Orientation.Right: return Orientation.Left;
        default: return Orientation.Up;
    }
}

int NbTurns(Orientation o1, Orientation o2)
{
    int res = Math.Abs((int)o1 - (int)o2);
    if (res == 3)
        return 1; // turn the other way / wrap around the enum
    return res;
}

static class Problem
{
    public const int Size = 141; 
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

    public static Pos operator+(Pos p1, Pos p2)
    {
        return new Pos
        {
            x = p1.x + p2.x,
            y = p1.y + p2.y
        };
    }
}

struct OrientedPos
{
    public Pos p;
    public Orientation o;

    public OrientedPos(Pos p, Orientation o)
    {
        this.p = p;
        this.o = o;
    }
}

enum Orientation
{
    Up,
    Right,
    Down,
    Left
}

struct CellState
{
    public bool IsWall;
    public bool IsReached;
    public bool AllNeighboursReached;
    public Orientation ReachedOrientation;
    public int Distance;

    public CellState()
    {
        IsWall = false;
        IsReached = false;
        ReachedOrientation = Orientation.Right;
        Distance = 0;
    }
}