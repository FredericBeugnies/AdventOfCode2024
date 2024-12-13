using System.Collections.Generic;
using System.Linq;

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
    //Console.WriteLine($"A region of {region.plant} plants with price {region.Area()} * {region.Perimeter()}");
    totalPrice += region.Area() * region.Perimeter();
}
Console.WriteLine($"Total Price: {totalPrice}");

// part 2

int totalPrice2 = 0;
foreach (var region in regions)
{
    Console.WriteLine($"Pos {region.plots[0].x}, {region.plots[0].y}: A region of {region.plant} plants with price {region.Area()} * {region.GetNbSides()}");
    totalPrice2 += region.Area() * region.GetNbSides();
}
Console.WriteLine($"Total Price - part 2: {totalPrice2}");

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

    public List<PlotEdge> GetPlotEdges()
    {
        return new List<PlotEdge>()
        {
            new PlotEdge(x, y, PlotEdge.Orientation.Up),
            new PlotEdge(x, y, PlotEdge.Orientation.Down),
            new PlotEdge(x, y, PlotEdge.Orientation.Left),
            new PlotEdge(x, y, PlotEdge.Orientation.Right),
        };
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

    public int GetNbSides()
    {
        HashSet<PlotEdge> regionEdges = [];
        foreach (var plot in plots)
        {
            var plotEdges = plot.GetPlotEdges();
            foreach (var edge in plotEdges)
            {
                bool adjacentToEdge = false;
                foreach (var regionEdge in regionEdges)
                {
                    if (regionEdge.IsAdjacentTo(edge))
                    {
                        adjacentToEdge = true;
                        regionEdges.Remove(regionEdge);
                        break;
                    }
                }
                if (!adjacentToEdge)
                {
                    regionEdges.Add(edge);
                }
            }
        }

        List<PlotEdge>[,] sortedEdges = new List<PlotEdge>[4, Problem.GridSize];
        foreach (var edge in regionEdges)
        {
            int o = (int)edge.orientation;
            int c = (edge.orientation == PlotEdge.Orientation.Up || edge.orientation == PlotEdge.Orientation.Down) ? edge.y : edge.x;
            if (sortedEdges[o, c] == null)
                sortedEdges[o, c] = new List<PlotEdge>();
            sortedEdges[o, c].Add(edge);
        }

        int totalNbSides = 0;
        for (int o = 0; o < 4; ++o)
        {
            for (int c = 0; c < Problem.GridSize; ++c)
            {
                if (sortedEdges[o, c] == null) continue;
                sortedEdges[o, c] = sortedEdges[o, c].OrderBy(s => s.x).ThenBy(s => s.y).ToList();
                int nbDiscontinuities = 0;
                for (int i = 1; i < sortedEdges[o, c].Count; ++i)
                {
                    if (o == (int)PlotEdge.Orientation.Up || o == (int)PlotEdge.Orientation.Down) // horizontal
                    {
                        if (sortedEdges[o, c][i].x != sortedEdges[o, c][i - 1].x + 1)
                            ++nbDiscontinuities;
                    }
                    else
                    {
                        if (sortedEdges[o, c][i].y != sortedEdges[o, c][i - 1].y + 1)
                            ++nbDiscontinuities;
                    }
                }

                totalNbSides += nbDiscontinuities + 1;
            }
        }

        return totalNbSides;
    }
}

struct PlotEdge
{
    public enum Orientation
    {
        Up,
        Down,
        Left,
        Right
    }

    public Orientation orientation;
    public int x;
    public int y;

    public PlotEdge(int x, int y, Orientation orientation)
    {
        this.x = x;
        this.y = y;
        this.orientation = orientation;
    }

    public bool IsAdjacentTo(PlotEdge other)
    {
        switch (orientation)
        {
            case Orientation.Up: return (other.orientation == Orientation.Down && other.x == x && other.y == y - 1);
            case Orientation.Down: return (other.orientation == Orientation.Up && other.x == x && other.y == y + 1);
            case Orientation.Left: return (other.orientation == Orientation.Right && other.x == x - 1 && other.y == y);
            case Orientation.Right: return (other.orientation == Orientation.Left && other.x == x + 1 && other.y == y);
            default: return false;
        }
    }
}