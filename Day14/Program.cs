using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");

// part 1
const int nbIterations = 100;
int[] q = new int[4];
foreach (var line in lines)
{
    var robot = Robot.Parse(line);

    for (int i = 0; i < nbIterations; i++)
        robot.DoMove();

    if (robot.px == Problem.NbCols / 2 || robot.py == Problem.NbRows / 2)
        continue;

    if (robot.px > Problem.NbCols / 2)
    {
        if (robot.py > Problem.NbRows / 2)
            q[0]++;
        else
            q[1]++;
    }
    else
    {
        if (robot.py > Problem.NbRows / 2)
            q[2]++;
        else
            q[3]++;
    }
}

int res = q[0] * q[1] * q[2] * q[3];
Console.WriteLine(res);

// part 2 - infinite loop with eyeball test :-)
List<Robot> robots = [];
foreach (var line in lines)
{
    robots.Add(Robot.Parse(line));
}

int elapsedSeconds = 0;
while (true)
{
    ++elapsedSeconds;

    foreach (var robot in robots)
        robot.DoMove();

    // display robot positions
    char[,] map = new char[Problem.NbCols, Problem.NbRows];
    for (int r = 0; r < Problem.NbRows; r++)
        for (int c = 0; c < Problem.NbCols; c++)
            map[c, r] = ' ';
    foreach (var robot in robots)
    {
        map[robot.px, robot.py] = '*';
    }
    if (HasStraightLine(map, 8))
    {
        Console.Clear();
        Console.WriteLine($"Candidate found - {elapsedSeconds} seconds elapsed");
        for (int r = 0; r < Problem.NbRows; r++)
        {
            for (int c = 0; c < Problem.NbCols; c++)
            {
                Console.Write(map[c, r]);
            }
            Console.WriteLine();
        }
        int i = 0; // put breakpoint here to manually check map
    }
}

bool HasStraightLine(char[,] map, int length)
{
    for (int r = 0; r < Problem.NbRows; r++)
        for (int c = 0; c < Problem.NbCols; c++)
            if (map[c, r] == '*')
            {
                for (int j = c + 1; j < Problem.NbCols; j++)
                {
                    if (map[j, r] != '*')
                        break;
                    if (j - c > length)
                        return true;
                }
            }
    return false;
}

static class Problem
{
    public const int NbRows = 103;
    public const int NbCols = 101;
}

class Robot
{
    public int px;
    public int py;
    public int dx;
    public int dy;

    static public Robot Parse(string line)
    {
        Robot robot = new();

        const string regex = @"p=(\d+),(\d+) v=(-?\d+),(-?\d+)";

        var matches = Regex.Matches(line, regex);
        robot.px = int.Parse(matches[0].Groups[1].Value);
        robot.py = int.Parse(matches[0].Groups[2].Value);
        robot.dx = int.Parse(matches[0].Groups[3].Value);
        robot.dy = int.Parse(matches[0].Groups[4].Value);

        return robot;
    }

    public void DoMove()
    {
        px += dx;
        if (px >= Problem.NbCols)
            px %= Problem.NbCols;
        if (px < 0)
            px += Problem.NbCols;

        py += dy;
        if (py >= Problem.NbRows)
            py %= Problem.NbRows;
        if (py < 0)
            py += Problem.NbRows;
    }
}