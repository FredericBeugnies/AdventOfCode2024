var lines = File.ReadAllLines("input.txt");

var input = lines[0].Select(c => int.Parse(c.ToString())).ToList();

int nbBlocks = 0;
input.ForEach(x => nbBlocks += x);

int[] blocks = new int[nbBlocks];
int blockId = 0;
int k = 0;
for (int i = 0; i < input.Count; ++i)
{
    int blockState = (i % 2 == 0) ? blockId++ : -1;
    for (int j = 0; j < input[i]; ++j)
        blocks[k++] = blockState;
}

// defrag
int l = 0;
int r = blocks.Length - 1;
while (l < r)
{
    while (l < blocks.Length && blocks[l] != -1) ++l;
    while (r >= 0 && blocks[r] == -1) --r;
    if (l < r)
    {
        blocks[l] = blocks[r];
        blocks[r] = -1;
    }
}

// compute checksum
long res = 0;
int pos = 0;
foreach (var item in blocks)
{
    if (item != -1)
       res += pos * item;
    ++pos;
}
Console.WriteLine(res);

// part 2
var zones = GetZoneList(input);
for (int zr = zones.Count - 1; zr >= 0; --zr)
{
    if (zones[zr].Type == ZoneType.File)
    {
        for (int zl = 0; zl < zr; ++zl)
        {
            if (zones[zl].Type == ZoneType.EmptySpace && zones[zl].Length >= zones[zr].Length)
            {
                // do the move
                zones[zl].Length -= zones[zr].Length;

                zones.Insert(zl, new Zone
                {
                    Type = ZoneType.File,
                    FileId = zones[zr].FileId,
                    Length = zones[zr].Length
                });

                ++zr;
                zones[zr].Type = ZoneType.EmptySpace;
                zones[zr].FileId = -1;

                break;
            }
        }
    }
}

int[] blocks2 = new int[nbBlocks];
k = 0;
for (int i = 0; i < zones.Count; ++i)
{
    for (int j = 0; j < zones[i].Length; ++j)
    {
        blocks2[k++] = zones[i].FileId;
    }
}

res = 0;
pos = 0;
foreach (var item in blocks2)
{
    if (item != -1)
        res += pos * item;
    ++pos;
}
Console.WriteLine(res);

List<Zone> GetZoneList(List<int> input)
{
    List<Zone> res = [];
    int fileId = 0;
    for (int i = 0; i < input.Count; ++i)
    {
        if (i % 2 == 0)
        {
            res.Add(new Zone
            {
                Type = ZoneType.File,
                FileId = fileId++,
                Length = input[i]
            });
        }
        else
        {
            res.Add(new Zone
            {
                Type = ZoneType.EmptySpace,
                FileId = -1,
                Length = input[i]
            });
        }
    }
    return res;
}

enum ZoneType
{
    File,
    EmptySpace
}

class Zone
{
    public ZoneType Type;
    public int FileId;
    public int Length;
}
