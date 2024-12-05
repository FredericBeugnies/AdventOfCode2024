var lines = File.ReadAllLines("input.txt");

int res = 0;
int res2 = 0;
for (int i = 0; i < lines.Length; i++)
{
    for (int j = 0; j < lines[i].Length; j++)
    {
        res += CountXmasOccurrences(lines, i, j);
        res2 += CountXmasOccurrences2(lines, i, j);
    }
}
Console.WriteLine(res);
Console.WriteLine(res2);

int CountXmasOccurrences(string[] lines, int i, int j)
{
    int res = 0;
    string mas = "MAS";
    if (lines[i][j] != 'X')
        return 0;

    for (int di = -1; di < 2; ++di)
    {
        for (int dj = -1; dj < 2; ++dj)
        {
            if (di == 0 && dj == 0) continue;
            int found = 1;
            for (int l = 0; l < mas.Length; ++l)
            {
                int ii = i + (l + 1) * di;
                int jj = j + (l + 1) * dj;
                if (ii < 0 || ii >= lines.Length) break;
                if (jj < 0 || jj >= lines[ii].Length) break;
                if (lines[i+(l+1)*di][j+(l+1)*dj] == mas[l])
                {
                    ++found;
                    //break;
                }
            }
            if (found == 4)
            {
                Console.WriteLine($"found {i} {j} {di} {dj}");
                ++res;
            }
        }
    }

    return res;
}

int CountXmasOccurrences2(string[] lines, int i, int j)
{
    if (lines[i][j] != 'A') return 0;
    if (i < 1) return 0;
    if (i >= lines.Length-1) return 0;
    if (j < 1) return 0;
    if (j >= lines[i].Length - 1) return 0;

    bool diag1a = (lines[i - 1][j - 1] == 'M' && lines[i + 1][j + 1] == 'S');
    bool diag1b = (lines[i - 1][j - 1] == 'S' && lines[i + 1][j + 1] == 'M');
    bool diag2a = (lines[i - 1][j + 1] == 'M' && lines[i + 1][j - 1] == 'S');
    bool diag2b = (lines[i - 1][j + 1] == 'S' && lines[i + 1][j - 1] == 'M');

    if ((diag1a || diag1b) && (diag2a || diag2b))
        return 1;

    return 0;
}