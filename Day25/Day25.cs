var input = File.ReadAllLines("input25.txt");

List<int[]> locks = [];
List<int[]> keys = [];
int i = 0;
while (i < input.Length)
{
    string[] matrix = input[i..(i + 6)];
    if (IsLock(matrix))
        locks.Add(GetHeights(matrix));
    else
        keys.Add(GetHeights(matrix));
    i += 8;
}

int res_part1 = 0;
foreach (int[] l in locks)
{
    foreach (int[] k in keys)
    {
        if (IsFit(l, k))
            ++res_part1;
    }
}
Console.WriteLine($"Part 1: {res_part1}");

bool IsLock(string[] matrix)
{
    return matrix[0] == "#####";
}

int[] GetHeights(string[] matrix)
{
    int[] res = new int[5];
    for (int i = 0; i < 5; ++i)
        for (int j = 1; j < 6; ++j)
            if (matrix[j][i] == '#')
                ++res[i];
    return res;
}

bool IsFit(int[] loc, int[] key)
{
    for (int i = 0; i < 5; ++i)
        if (loc[i] + key[i] > 5)
            return false;
    return true;
}