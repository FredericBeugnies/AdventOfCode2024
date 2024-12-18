using System.Text;

var lines = File.ReadAllLines("input.txt");

// parse map
Pos robot = default;
char[,] map = new char[Problem.GridSize, Problem.GridSize];
for (int r = 0; r < Problem.GridSize; r++)
{
    for (int c = 0; c < Problem.GridSize; c++)
    {
        map[c, r] = lines[r][c];
        if (map[c, r] == '@')
        {
            robot = new Pos(c, r);
        }
    }
}

// parse moves
StringBuilder sb = new StringBuilder();
for (int i = Problem.GridSize; i < lines.Length; i++)
{
    sb.Append(lines[i]);
}
string moves = sb.ToString();

// play the moves
foreach (char move in moves)
{
    Pos delta;
    switch (move)
    {
        case '^': delta = new Pos(0, -1); break;
        case '<': delta = new Pos(-1, 0); break;
        case '>': delta = new Pos(1, 0); break;
        case 'v': delta = new Pos(0, 1); break;
        default: delta = new Pos(0, 0); break;
    }

    Pos newRobotPos = robot + delta;

    if (map[newRobotPos.x, newRobotPos.y] == '.')
    {
        // next pos is empty, do the move
        map[newRobotPos.x, newRobotPos.y] = '@';
        map[robot.x, robot.y] = '.';
        robot = newRobotPos;
    }
    else if (map[newRobotPos.x, newRobotPos.y] == 'O')
    {
        // search for a space in the appropriate direction
        Pos s = newRobotPos;
        while (map[s.x, s.y] != '#' && map[s.x, s.y] != '.')
        {
            s += delta;
        }
        if (map[s.x, s.y] == '.')
        {
            // space found, do the move
            map[s.x, s.y] = 'O';
            map[newRobotPos.x, newRobotPos.y] = '@';
            map[robot.x, robot.y] = '.';
            robot = newRobotPos;
        }
        else
        {
            // no move
        }
    }
    else
    {
        // no move
    }
}

Display(map);

// compute sum of gps coords
long sum = 0;
for (int y = 0; y < Problem.GridSize; ++y)
    for (int x = 0; x < Problem.GridSize; ++x)
        if (map[x, y] == 'O')
            sum += x + 100 * y;

Console.WriteLine($"Part 1: {sum}");

void Display(char[,] map)
{
    Console.Clear();
    for (int y = 0; y < Problem.GridSize; ++y)
    {
        for (int x = 0; x < Problem.GridSize; ++x)
        {
            Console.Write(map[x, y]);
        }
        Console.WriteLine();
    }
}

static class Problem
{
    public const int GridSize = 50;
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

    public static Pos operator+(Pos a, Pos b)
    {
        return new Pos(a.x + b.x, a.y + b.y);
    }
}