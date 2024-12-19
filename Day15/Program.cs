using System.Text;

var lines = File.ReadAllLines("input.txt");

Part1(lines);
Part2(lines);

void Part1(string[] lines)
{
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

    string moves = ParseMoves(lines);

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

    //Display(map);

    // compute sum of gps coords
    long sum = 0;
    for (int y = 0; y < Problem.GridSize; ++y)
        for (int x = 0; x < Problem.GridSize; ++x)
            if (map[x, y] == 'O')
                sum += x + 100 * y;

    Console.WriteLine($"Part 1: {sum}");
}

// part 2
void Part2(string[] lines)
{
    // parse double-witdh map
    Pos robot = default;
    char[,] map2 = new char[2 * Problem.GridSize, Problem.GridSize];
    for (int r = 0; r < Problem.GridSize; r++)
    {
        for (int c = 0; c < Problem.GridSize; c++)
        {
            switch (lines[r][c])
            {
                case 'O':
                    map2[2 * c, r] = '[';
                    map2[2 * c + 1, r] = ']';
                    break;
                case '@':
                    map2[2 * c, r] = '@';
                    map2[2 * c + 1, r] = '.';
                    robot = new Pos(2 * c, r);
                    break;
                default:
                    map2[2 * c, r] = lines[r][c];
                    map2[2 * c + 1, r] = lines[r][c];
                    break;
            }
        }
    }

    string moves = ParseMoves(lines);

    //Display2(map2);

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

        if (map2[newRobotPos.x, newRobotPos.y] == '.')
        {
            // next pos is empty, do the move
            map2[newRobotPos.x, newRobotPos.y] = '@';
            map2[robot.x, robot.y] = '.';
            robot = newRobotPos;
        }
        else if (map2[newRobotPos.x, newRobotPos.y] == '[' || map2[newRobotPos.x, newRobotPos.y] == ']')
        {
            if (move == '<' || move == '>')
            {
                // horizontal move
                // search for a space in the appropriate direction
                Pos s = newRobotPos;
                while (map2[s.x, s.y] != '#' && map2[s.x, s.y] != '.')
                {
                    s += delta;
                }
                if (map2[s.x, s.y] == '.')
                {
                    // space found, do the move
                    if (move == '<')
                    {
                        for (int i = s.x; i < newRobotPos.x; i++)
                            map2[i, s.y] = map2[i + 1, s.y];
                    }
                    else
                    {
                        for (int i = s.x; i > newRobotPos.x; i--)
                            map2[i, s.y] = map2[i - 1, s.y];
                    }
                    map2[newRobotPos.x, newRobotPos.y] = '@';
                    map2[robot.x, robot.y] = '.';
                    robot = newRobotPos;
                }
                else
                {
                    // boxes touch the wall => no move
                }
            }
            else
            {
                // vertical move
                // find out all the boxes that would be pushed together
                HashSet<Pos> movingBoxes = [];
                Queue<Pos> q = [];
                q.Enqueue(newRobotPos);
                bool wallFound = false;
                while (q.Count > 0)
                {
                    Pos p = q.Dequeue();
                    if (map2[p.x, p.y] == '[')
                    {
                        movingBoxes.Add(p);
                        q.Enqueue(p + delta);
                        q.Enqueue(p + delta + new Pos(1, 0));
                    }
                    else if (map2[p.x, p.y] == ']')
                    {
                        movingBoxes.Add(new Pos(p.x - 1, p.y));
                        q.Enqueue(p + delta);
                        q.Enqueue(p + delta + new Pos(-1, 0));
                    }
                    else if (map2[p.x, p.y] == '#')
                    {
                        wallFound = true;
                    }
                }

                if (!wallFound)
                {
                    Dictionary<int, List<Pos>> boxesByLevel = [];
                    foreach (Pos p in movingBoxes)
                    {
                        if (!boxesByLevel.ContainsKey(p.y))
                            boxesByLevel[p.y] = [];
                        boxesByLevel[p.y].Add(p);
                    }

                    List<int> levels;
                    if (delta.y > 0)
                        levels = boxesByLevel.Keys.OrderByDescending(y => y).ToList();
                    else
                        levels = boxesByLevel.Keys.OrderBy(y => y).ToList();

                    // move the boxes
                    foreach (int level in levels)
                    {
                        foreach (Pos p in boxesByLevel[level])
                        {
                            Pos p2 = p + delta;
                            map2[p2.x, p2.y] = map2[p.x, p.y];
                            map2[p2.x + 1, p2.y] = map2[p.x + 1, p.y];
                            map2[p.x, p.y] = '.';
                            map2[p.x + 1, p.y] = '.';
                        }
                    }

                    map2[newRobotPos.x, newRobotPos.y] = '@';
                    map2[robot.x, robot.y] = '.';
                    robot = newRobotPos;
                }
                else
                {
                    // one of the boxes touches a wall in the move direction => no move
                }
            }
        }
        else
        {
            // robot against a wall => no move
        }

        //Display2(map2);
    }

    // compute sum of gps coords
    long sum = 0;
    for (int y = 0; y < Problem.GridSize; ++y)
        for (int x = 0; x < 2 * Problem.GridSize; ++x)
            if (map2[x, y] == '[')
                sum += x + 100 * y;

    Console.WriteLine($"Part 2: {sum}");
}

string ParseMoves(string[] lines)
{
    StringBuilder sb = new StringBuilder();
    for (int i = Problem.GridSize; i < lines.Length; i++)
    {
        sb.Append(lines[i]);
    }
    return sb.ToString();
}

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

void Display2(char[,] map)
{
    Console.Clear();
    for (int y = 0; y < Problem.GridSize; ++y)
    {
        for (int x = 0; x < 2 * Problem.GridSize; ++x)
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