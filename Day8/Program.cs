var lines = File.ReadAllLines("input.txt");

Dictionary<char, List<Pos>> antennas = [];
for (int l = 0; l < lines.Length; l++)
{
    for (int c = 0; c < lines[l].Length; c++)
    {
        if (lines[l][c] != '.')
        {
            char freq = lines[l][c];
            if (!antennas.ContainsKey(freq))
            {
                antennas.Add(freq, []);
            }
            antennas[freq].Add(new Pos(c, l));
        }
    }
}

// part 1
Grid antinodes = new();
foreach (var freq in antennas.Keys)
{
    // iterate through pairs of antennas
    for (int i = 0; i < antennas[freq].Count; i++)
    {
        for (int j = i + 1; j < antennas[freq].Count; j++)
        {
            Pos pi = antennas[freq][i];
            Pos pj = antennas[freq][j];
            int dx = pj.x - pi.x;
            int dy = pj.y - pi.y;
            Pos antinode1 = new Pos(pj.x + dx, pj.y + dy);
            Pos antinode2 = new Pos(pi.x - dx, pi.y - dy);
            if (!antinode1.OutOfBounds())
                antinodes.m[antinode1.x, antinode1.y] = '#';
            if (!antinode2.OutOfBounds())
                antinodes.m[antinode2.x, antinode2.y] = '#';
        }
    }
}

int result = 0;
for (int y = 0; y < Problem.Size; y++)
    for (int x = 0; x < Problem.Size; x++)
        if (antinodes.m[x, y] == '#')
            ++result;

Console.WriteLine(result);

// part 2
Grid antinodes2 = new();
foreach (var freq in antennas.Keys)
{
    // iterate through pairs of antennas
    for (int i = 0; i < antennas[freq].Count; i++)
    {
        for (int j = i + 1; j < antennas[freq].Count; j++)
        {
            Pos pi = antennas[freq][i];
            Pos pj = antennas[freq][j];
            int dx = pj.x - pi.x;
            int dy = pj.y - pi.y;
            
            // find GCD of dx, dy
            for (int div = Math.Min(dx, dy); div > 1; div--)
            {
                if (dx % div == 0 && dy % div == 0)
                {
                    dx /= div;
                    dy /= div;
                    break;
                }
            }

            antinodes2.m[pi.x, pi.y] = '#';
            Pos p = new Pos(pi.x, pi.y);
            while (true)
            {
                p.x -= dx;
                p.y -= dy;
                if (p.OutOfBounds())
                    break;
                antinodes2.m[p.x, p.y] = '#';
            }
            p = new Pos(pi.x, pi.y);
            while (true)
            {
                p.x += dx;
                p.y += dy;
                if (p.OutOfBounds())
                    break;
                antinodes2.m[p.x, p.y] = '#';
            }
        }
    }
}

int result2 = 0;
for (int y = 0; y < Problem.Size; y++)
    for (int x = 0; x < Problem.Size; x++)
        if (antinodes2.m[x, y] == '#')
            ++result2;

Console.WriteLine(result2);

static class Problem
{
    public const int Size = 50;
}

class Pos
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
    public char[,] m = new char[Problem.Size, Problem.Size];

    public Grid()
    {
        for (int x = 0; x < Problem.Size; x++)
            for (int y = 0; y < Problem.Size; y++)
                m[x, y] = default;
    }
}
