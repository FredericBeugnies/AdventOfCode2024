var lines = File.ReadAllLines("input.txt");

Part1(lines);

void Part1(string[] lines)
{
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

    CloseDeadEnds(grid);

    CellState[,] state = new CellState[Problem.Size, Problem.Size];
    List<Pos> reached = [];
    for (int y = 0; y < lines.Length; y++)
        for (int x = 0; x < lines[y].Length; x++)
        {
            state[x, y] = new CellState();
            if (grid[x, y] == '#') state[x, y].IsWall = true;
        }
    state[start.x, start.y].IsReached = true;
    reached.Add(start);

    DisplayGrid(grid);

    // compute shortest path
    do
    {
        int minDistance = Int32.MaxValue;
        OrientedPos closestNeighbour = default;

        // evaluate possible moves
        foreach (var pos in reached)
        {
            bool removePosFromActiveSet = true;
            foreach (var neighbour in GetNeighbours(pos))
            {
                var neighbourState = state[neighbour.p.x, neighbour.p.y];
                if (neighbourState.IsWall || neighbourState.IsReached)
                    continue;
                removePosFromActiveSet = false;

                // compute distance from pos to neighbour
                int distance = state[pos.x, pos.y].Distance + 1 + NbTurns(state[pos.x, pos.y].ReachedOrientation, neighbour.o) * 1000;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNeighbour = neighbour;
                }
            }
        }

        reached.Add(closestNeighbour.p);
        state[closestNeighbour.p.x, closestNeighbour.p.y].IsReached = true;
        state[closestNeighbour.p.x, closestNeighbour.p.y].Distance = minDistance;
        state[closestNeighbour.p.x, closestNeighbour.p.y].ReachedOrientation = closestNeighbour.o;
        if (closestNeighbour.p.x == end.x && closestNeighbour.p.y == end.y)
        {
            break;
        }
    } while (true);

    int res = state[end.x, end.y].Distance;
    Console.WriteLine($"Part 1: {res}");
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

void DisplayGrid(char[,] grid)
{
    for (int y = 0; y < grid.GetLength(0); y++)
    {
        for (int x = 0; x < grid.GetLength(1); x++)
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