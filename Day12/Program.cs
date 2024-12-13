using System.Collections.Generic;

var lines = File.ReadAllLines("input.txt");

// read input into 2D grid
char[,] map = new char[Problem.GridSize, Problem.GridSize];
for (int r = 0; r < lines.Count(); r++)
    for (int c = 0; c < lines[r].Count(); c++)
        map[c, r] = lines[r][c];

// explore grid to identify regions
List<Region> regions = new List<Region>();
bool[,] visited = new bool[Problem.GridSize, Problem.GridSize];
for (int x = 0; x < Problem.GridSize; x++)
    for (int y = 0; y < Problem.GridSize; y++)
    {
        if (visited[x, y]) continue;

        Region region = new Region
        {
            plant = map[x, y]
        };
        Queue<Plot> q = new Queue<Plot>();
        q.Enqueue(new Plot(x, y));
        while (q.Count > 0) 
        {
            Plot p = q.Dequeue();
            if (!OutOfBounds(p.x, p.y) && 
                !visited[p.x, p.y] && 
                map[p.x, p.y] == region.plant)
            {
                visited[p.x, p.y] = true;
                region.plots.Add(p);
                q.Enqueue(p.Up());
                q.Enqueue(p.Down());
                q.Enqueue(p.Left());
                q.Enqueue(p.Right());
            }
        }
        regions.Add(region);
    }

int totalPrice = 0;
foreach (var region in regions)
{
    Console.WriteLine($"A region of {region.plant} plants with price {region.Area()} * {region.Perimeter()}");
    totalPrice += region.Area() * region.Perimeter();
}
Console.WriteLine($"Total Price: {totalPrice}");

bool OutOfBounds(int x, int y)
{
    return x < 0 || y < 0 || x >= Problem.GridSize || y >= Problem.GridSize;
}

static class Problem
{
    public const int GridSize = 140;
}

struct Plot
{
    public int x;
    public int y;

    public Plot(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Plot Up()
    {
        return new Plot(x, y - 1);
    }

    public Plot Down()
    {
        return new Plot(x, y + 1);
    }

    public Plot Left()
    {
        return new Plot(x - 1, y);
    }

    public Plot Right()
    {
        return new Plot(x + 1, y);
    }

    public bool IsAdjacent(Plot other)
    {
        return (x == other.x && Math.Abs(y - other.y) == 1) || (y == other.y && Math.Abs(x - other.x) == 1);
    }
}

class Region
{
    public char plant;
    public List<Plot> plots = [];

    public int Area()
    {
        return plots.Count;
    }

    public int Perimeter()
    {
        int nbAdjacentPlotPairs = 0;
        for (int i = 0; i < plots.Count; i++) 
        { 
            for (int j = i + 1; j < plots.Count; j++)
            {
                if (plots[i].IsAdjacent(plots[j]))
                    nbAdjacentPlotPairs++;
            }
        }

        return plots.Count * 4 - nbAdjacentPlotPairs * 2;
    }
}