var lines = File.ReadAllLines("input.txt");

var matrix = new PrecedencyMatrix();

int result = 0;
int result_reordered = 0;
int i = 0;
while (!string.IsNullOrEmpty(lines[i]))
{
    var tokens = lines[i].Split('|', StringSplitOptions.RemoveEmptyEntries);
    int x = int.Parse(tokens[0]);
    int y = int.Parse(tokens[1]);
    matrix.SetPrecedency(x, y);
    ++i;
}
for (++i; i < lines.Length; ++i) // skip blank line
{
    // parse
    var tokens = lines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
    List<int> pages = [];
    for (int j = 0; j < tokens.Length; ++j)
    {
        pages.Add(int.Parse(tokens[j]));
    }

    // check if correctly ordered
    // add middle-page number
    if (HasCorrectOrder(pages, matrix))
        result += pages[pages.Count / 2];
    else
    {
        // re-order
        for (int j = 0; j < pages.Count - 1; ++j)
        {
            int maxPosWithPrecedency = 0;
            for (int k = j + 1; k < pages.Count; ++k)
            {
                if (matrix.HasPrecedency(pages[k], pages[j]))
                {
                    maxPosWithPrecedency = k;
                }
            }
            if (maxPosWithPrecedency > 0)
            {
                int value = pages[j];
                pages.RemoveAt(j);
                pages.Insert(maxPosWithPrecedency, value);
                //for (int k = 0; k < pages.Count; ++k)
                //    Console.Write($"{pages[k]} ");
                //    Console.WriteLine();
                --j; // value in pos j has changed, we need to re-check it
            }
        }

        result_reordered += pages[pages.Count / 2];
    }
}

Console.WriteLine($"Part1: {result}");
Console.WriteLine($"Part2: {result_reordered}");

bool HasCorrectOrder(List<int> pages, PrecedencyMatrix matrix)
{
    for (int j = 0; j < pages.Count - 1; ++j)
    {
        for (int k = j + 1; k < pages.Count; ++k)
        {
            if (matrix.HasPrecedency(pages[k], pages[j]))
            {
                //Console.WriteLine($"Line {i} incorrect: {pages[j]} {pages[k]}");
                return false;
            }
        }
    }

    return true;
}

class PrecedencyMatrix
{
    public const int Size = 100;
    private bool[,] matrix = new bool[Size, Size];

    public PrecedencyMatrix()
    {
        for (int i = 0; i < Size; i++) 
            for (int j = 0; j < Size; j++)
                matrix[i,j] = false;
    }

    public void SetPrecedency(int x, int y)
    {
        matrix[x,y] = true;
    }

    public bool HasPrecedency(int x, int y)
    {
        return matrix[x,y];
    }
};

