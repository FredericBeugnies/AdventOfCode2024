using System.Text;

var input = File.ReadAllLines("input21.txt");
//var input = File.ReadAllLines("sample21.txt");

const int nbNumKeys = 11;
// combos*[i, j] = shortest directional combos allowing to press numkey 'j' when starting in front of numkey 'i'
List<string>[,] combos = new List<string>[nbNumKeys, nbNumKeys]; // from robot2 perspective
List<string>[,] combos2 = new List<string>[nbNumKeys, nbNumKeys]; // from robot3 perspective
List<string>[,] combos3 = new List<string>[nbNumKeys, nbNumKeys]; // from human operator perspective

// compute 1st level combos
for (int i = 0; i < nbNumKeys; i++)
{
    for (int j = 0; j < nbNumKeys; j++)
    {
        combos[i, j] = GetAllValidPaths(i, j);
    }
}

// compute 2nd level combos
var table = GetTranslationTableForDirKeys();
for (int i = 0; i < nbNumKeys; i++)
{
    for (int j = 0; j < nbNumKeys; j++)
    {
        combos2[i, j] = [];
        foreach (var srcCombo in combos[i, j])
        {
            List<string> l2 = [""];
            int padIndex = GetIndexForDirKey('A');
            for (int k = 0; k < srcCombo.Length; k++)
            {
                List<string> l3 = [];
                int nextIndex = GetIndexForDirKey(srcCombo[k]);
                var translations = table[padIndex, nextIndex];
                foreach (var baseCombo in l2)
                {
                    foreach (var translation in translations)
                    {
                        l3.Add(baseCombo + translation);
                    }
                }
                l2 = l3;
                padIndex = nextIndex;
            }
            combos2[i, j].AddRange(l2);
        }
    }
}

// compute 3nd level combos
for (int i = 0; i < nbNumKeys; i++)
{
    for (int j = 0; j < nbNumKeys; j++)
    {
        combos3[i, j] = [];
        foreach (var srcCombo in combos2[i, j])
        {
            List<string> l2 = [""];
            int padIndex = GetIndexForDirKey('A');
            for (int k = 0; k < srcCombo.Length; k++)
            {
                List<string> l3 = [];
                int nextIndex = GetIndexForDirKey(srcCombo[k]);
                var translations = table[padIndex, nextIndex];
                foreach (var baseCombo in l2)
                {
                    foreach (var translation in translations)
                    {
                        l3.Add(baseCombo + translation);
                    }
                }
                l2 = l3;
                padIndex = nextIndex;
            }
            combos3[i, j].AddRange(l2);
        }
    }
}

int res = 0;
foreach (var numCombo in input)
{
    StringBuilder sb = new();
    int index = GetIndexForNumKay('A');
    for (int k = 0; k < numCombo.Length; k++)
    {
        int ii = GetIndexForNumKay(numCombo[k]);
        sb.Append(combos3[index, ii].OrderBy(x => x.Length).First());
        index = ii;
    }
    string resCombo = sb.ToString();
    
    int numValue = int.Parse(numCombo.Remove(numCombo.Length - 1));
    res += resCombo.Length * numValue;

    Console.WriteLine(numCombo);
    Console.WriteLine(resCombo);
}
Console.WriteLine($"Part 1: {res}");

List<string> GetAllValidPaths(int i, int j)
{
    List<string> res = [];
    Pos pi = GetPosForNumKeyIndex(i);
    Pos pj = GetPosForNumKeyIndex(j);
    int dx = pj.x - pi.x;
    int dy = pj.y - pi.y;
    char cx = dx > 0 ? '>' : '<';
    char cy = dy > 0 ? '^' : 'v';
    Pos xmove = dx > 0 ? new Pos(1, 0) : new Pos(-1, 0);
    Pos ymove = dy > 0 ? new Pos(0, 1) : new Pos(0, -1);
    dx = Math.Abs(dx);
    dy = Math.Abs(dy);

    List<Path> current = [];
    current.Add(new Path
    {
        builtPath = "",
        rx = Math.Abs(dx),
        ry = Math.Abs(dy),
        pos = pi
    });
    for (int k = 0; k < dx + dy; ++k)
    {
        List<Path> next = [];
        foreach (var p in current)
        {
            if (p.rx > 0)
            {
                Pos newPos = p.pos + xmove;
                if (newPos.x != 0 || newPos.y != 0)
                {
                    next.Add(new Path
                    {
                        builtPath = p.builtPath + cx,
                        rx = p.rx - 1,
                        ry = p.ry,
                        pos = p.pos + xmove
                    });
                }
            }
            if (p.ry > 0)
            {
                Pos newPos = p.pos + ymove;
                if (newPos.x != 0 || newPos.y != 0)
                {
                    next.Add(new Path
                    {
                        builtPath = p.builtPath + cy,
                        rx = p.rx,
                        ry = p.ry - 1,
                        pos = p.pos + ymove
                    });
                }
            }
        }
        current = next;
    }
    foreach (var p in current)
    {
        res.Add(p.builtPath + 'A');
    }
    return res;
}

string GetDirectionsForNumCombo(string numCombo)
{
    StringBuilder sb = new();
    Pos currentPos = GetPosForNumKey('A');
    foreach (char c in numCombo)
    {
        Pos nextPos = GetPosForNumKey(c);
        int dx = nextPos.x - currentPos.x;
        int dy = nextPos.y - currentPos.y;
        char cx = dx > 0 ? '>' : '<';
        char cy = dy > 0 ? 'v' : '^';
        string xmove = new string(cx, Math.Abs(dx));
        string ymove = new string(cy, Math.Abs(dy));
        for (int i = currentPos.x; i < nextPos.x; ++i)
            sb.Append('>');
        for (int i = currentPos.y; i > nextPos.y; --i)
            sb.Append('v');
        for (int i = currentPos.y; i < nextPos.y; ++i)
            sb.Append('^');
        for (int i = currentPos.x; i > nextPos.x; --i)
            sb.Append('<');
        sb.Append('A');
        currentPos = nextPos;
    }
    return sb.ToString();
}

string GetDirectionsForDirCombo(string numCombo)
{
    StringBuilder sb = new();
    Pos currentPos = GetPosForDirKey('A');
    foreach (char c in numCombo)
    {
        Pos nextPos = GetPosForDirKey(c);
        for (int i = currentPos.x; i < nextPos.x; ++i)
            sb.Append('>');
        for (int i = currentPos.y; i > nextPos.y; --i)
            sb.Append('v');
        for (int i = currentPos.y; i < nextPos.y; ++i)
            sb.Append('^');
        for (int i = currentPos.x; i > nextPos.x; --i)
            sb.Append('<');
        sb.Append('A');
        currentPos = nextPos;
    }
    return sb.ToString();
}

Pos GetPosForNumKey(char key)
{
    switch (key)
    {
        case 'A': return new Pos(2, 0);
        case '0': return new Pos(1, 0);
        case '1': return new Pos(0, 1);
        case '2': return new Pos(1, 1);
        case '3': return new Pos(2, 1);
        case '4': return new Pos(0, 2);
        case '5': return new Pos(1, 2);
        case '6': return new Pos(2, 2);
        case '7': return new Pos(0, 3);
        case '8': return new Pos(1, 3);
        case '9': return new Pos(2, 3);
        default: return new Pos(0, 0); // should never happen
    }
}

Pos GetPosForNumKeyIndex(int numKeyIndex)
{
    switch (numKeyIndex)
    {
        case 0: return new Pos(1, 0);
        case 1: return new Pos(0, 1);
        case 2: return new Pos(1, 1);
        case 3: return new Pos(2, 1);
        case 4: return new Pos(0, 2);
        case 5: return new Pos(1, 2);
        case 6: return new Pos(2, 2);
        case 7: return new Pos(0, 3);
        case 8: return new Pos(1, 3);
        case 9: return new Pos(2, 3);
        case 10: return new Pos(2, 0); // 'A'
        default: return new Pos(0, 0); // should never happen
    }
}

Pos GetPosForDirKey(char key)
{
    switch (key)
    {
        case 'A': return new Pos(2, 1);
        case '^': return new Pos(1, 1);
        case '<': return new Pos(0, 0);
        case 'v': return new Pos(1, 0);
        case '>': return new Pos(2, 0);
        default: return new Pos(0, 0); // should never happen
    }
}

List<string>[,] GetTranslationTableForDirKeys()
{
    List<string>[,] res = new List<string>[5, 5];

    res[0, 0] = ["A"];
    res[0, 1] = ["v<A"];
    res[0, 2] = ["vA"];
    res[0, 3] = ["v>A", ">vA"];
    res[0, 4] = [">A"];
    
    res[1, 0] = [">^A"];
    res[1, 1] = ["A"];
    res[1, 2] = [">A"];
    res[1, 3] = [">>A"];
    res[1, 4] = [">>^A", ">^>A"];
    
    res[2, 0] = ["^A"];
    res[2, 1] = ["<A"];
    res[2, 2] = ["A"];
    res[2, 3] = [">A"];
    res[2, 4] = [">^A", "^>A"];
    
    res[3, 0] = ["<^A", "^<A"];
    res[3, 1] = ["<<A"];
    res[3, 2] = ["<A"];
    res[3, 3] = ["A"];
    res[3, 4] = ["^A"];
    
    res[4, 0] = ["<A"];
    res[4, 1] = ["v<<A", "<v<A"];
    res[4, 2] = ["<vA", "v<A"];
    res[4, 3] = ["vA"];
    res[4, 4] = ["A"];

    return res;
}

int GetIndexForNumKay(char numKey)
{
    if (numKey == 'A')
        return 10;
    else
        return int.Parse(numKey.ToString());
}

int GetIndexForDirKey(char dirKey)
{
    switch(dirKey) 
    {
        case '^': return 0;
        case '<': return 1;
        case 'v': return 2;
        case '>': return 3;
        case 'A': return 4;
        default: return 0; // should never happen
    }
}

struct Path
{
    public string builtPath;
    public int rx;
    public int ry;
    public Pos pos;
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

    static public Pos operator +(Pos left, Pos right)
    {
        return new Pos
        {
            x = left.x + right.x,
            y = left.y + right.y
        };
    }
}