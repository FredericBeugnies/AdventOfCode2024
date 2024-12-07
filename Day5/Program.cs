var lines = File.ReadAllLines("input.txt");

var matrix = new PrecedencyMatrix();

int i = 0;
while (!string.IsNullOrEmpty(lines[i]))
{
    var tokens = lines[i].Split('|', StringSplitOptions.RemoveEmptyEntries);
    int x = int.Parse(tokens[0]);
    int y = int.Parse(tokens[1]);
    matrix.SetPrecedency(x, y);
    ++i;
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

