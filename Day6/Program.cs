var lines = File.ReadAllLines("input.txt");

const int size = 130;
//char[,] map = new char[size, size];
char[,] visited = new char[size, size];
Grid originalMap = new();

// parse map and search initial position
var startPos = new Pos();
for (int i = 0; i < size; i++)
    for (int j = 0; j < size; j++)
    {
        originalMap.m[j, i] = lines[i][j];
        visited[j, i] = default;
        if (IsGuard(originalMap.m, j, i))
            startPos = new Pos(j, i);
    }

// do the trip
bool stop = false;
Pos currentPos = new(startPos);
do
{
    visited[currentPos.x, currentPos.y] = 'X';

    // compute next pos
    var nextPos = new Pos();
    switch (originalMap.m[currentPos.x, currentPos.y])
    {
        case '^': nextPos = currentPos.Up(); break;
        case '<': nextPos = currentPos.Left(); break;
        case '>': nextPos = currentPos.Right(); break;
        case 'v': nextPos = currentPos.Down(); break;
    }

    if (nextPos.OutOfBounds())
    {
        stop = true;
        continue;
    }
    
    if (IsObstacle(originalMap.m, nextPos.x, nextPos.y))
    {
        TurnRight(originalMap.m, currentPos.x, currentPos.y);
    }
    else
    {
        // advance
        originalMap.m[nextPos.x, nextPos.y] = originalMap.m[currentPos.x, currentPos.y];
        currentPos = nextPos;
    }
} while (!stop);

// count the X's
List<Pos> originalTrip = [];
for (int i = 0; i < size; i++)
    for (int j = 0; j < size; j++) 
        if (visited[j, i] == 'X')
            originalTrip.Add(new Pos(j, i));

Console.WriteLine(originalTrip.Count());

// part 2
int count2 = 0;
foreach (Pos pos in originalTrip)
{
    if (pos.x == startPos.x && pos.y == startPos.y) continue;

    // create copy of the original map
    Grid altMap = new(originalMap);

    // try putting an obstacle in 'pos'
    altMap.m[pos.x, pos.y] = '#';

    // do the trip and detect if this results in a cycle
    CycleDetector cd = new();
    bool stop2 = false;
    bool cycleDetected = false;
    Pos curPos = new(startPos);
    do
    {
        visited[curPos.x, curPos.y] = 'X';

        // compute next pos
        var nextPos = new Pos();
        switch (altMap.m[curPos.x, curPos.y])
        {
            case '^': nextPos = curPos.Up(); break;
            case '<': nextPos = curPos.Left(); break;
            case '>': nextPos = curPos.Right(); break;
            case 'v': nextPos = curPos.Down(); break;
        }

        if (nextPos.OutOfBounds())
        {
            stop2 = true;
            continue;
        }

        if (IsObstacle(altMap.m, nextPos.x, nextPos.y))
        {
            TurnRight(altMap.m, curPos.x, curPos.y);
        }
        else
        {
            // advance
            altMap.m[nextPos.x, nextPos.y] = altMap.m[curPos.x, curPos.y];
            curPos = nextPos;
            if (cd.AddVisit(curPos, altMap.m[curPos.x, curPos.y]))
            {
                // cycle detected
                cycleDetected = true;
                stop2 = true;
            }
        }
    } while (!stop2);
    if (cycleDetected)
        ++count2;
}
Console.WriteLine(count2);

bool IsGuard(char[,] map, int x, int y)
{
    char[] possibleRepresentations = ['<', '>', '^', 'v'];
    return possibleRepresentations.Contains(map[x, y]);
}

bool IsObstacle(char[,] map, int x, int y)
{
    return map[x, y] == '#';
}

void TurnRight(char[,] map, int x, int y)
{
    switch (map[x, y])
    {
        case '^': map[x, y] = '>'; break;
        case '>': map[x, y] = 'v'; break;
        case 'v': map[x, y] = '<'; break;
        case '<': map[x, y] = '^'; break;
    }
}

static class Problem
{
    public const int NbRows = 130;
    public const int NbCols = 130;
}

class Pos
{
    public int x;
    public int y;

    public Pos() : this(0, 0) { }

    public Pos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Pos(Pos other)
    {
        this.x = other.x;
        this.y = other.y;
    }

    public bool OutOfBounds()
    {
        return x < 0 || y < 0 || x >= Problem.NbCols || y >= Problem.NbRows;
    }

    public Pos Up()
    {
        return new Pos(x, y - 1);
    }

    public Pos Down()
    {
        return new Pos(x, y + 1);
    }

    public Pos Left()
    {
        return new Pos(x - 1, y);
    }

    public Pos Right()
    {
        return new Pos(x + 1, y);
    }
}

class Grid
{
    public char[,] m = new char[Problem.NbCols, Problem.NbRows];

    public Grid()
    {
        for (int x = 0; x < Problem.NbCols; x++)
            for (int y = 0; y < Problem.NbRows; y++)
                m[x, y] = default;
    }

    public Grid(Grid other)
    {
        for (int x = 0; x < Problem.NbCols; x++)
            for (int y = 0; y < Problem.NbRows; y++)
                this.m[x, y] = other.m[x, y];
    }
}

class CycleDetector
{
    // m[x,y,z] is true iff cell(x,y) has been visited with orientation 'z'
    public bool[,,] m = new bool[Problem.NbCols, Problem.NbRows, 4];

    public CycleDetector()
    {
        for (int x = 0; x < Problem.NbCols; x++)
            for (int y = 0; y < Problem.NbRows; y++)
                for (int z = 0; z < 4; ++z)
                    m[x, y, z] = false;
    }

    public bool AddVisit(Pos pos, char orientation)
    {
        int z = GetOrientationValue(orientation);
        if (m[pos.x, pos.y, z])
            return true;
        m[pos.x, pos.y, z] = true;
        return false;
    }

    private int GetOrientationValue(char orientation)
    {
        switch (orientation) 
        {
            case '^': return 0;
            case '>': return 1;
            case 'v': return 2;
            case '<': return 3;
            default:  return 0;
        }
    }
}