List<int> list1 = [];
List<int> list2 = [];

string[] lines = File.ReadAllLines("input.txt");
foreach (string line in lines)
{
    var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    list1.Add(Int32.Parse(tokens[0]));
    list2.Add(Int32.Parse(tokens[1]));
}

list1 = list1.Order().ToList();
list2 = list2.Order().ToList();

// part 1
long result = 0;
for (int i = 0; i < list1.Count; ++i)
{
    result += Math.Abs(list1[i] - list2[i]);
}

Console.WriteLine(result);

// part 2
var occ1 = ComputeOccurrenceTable(list1);
var occ2 = ComputeOccurrenceTable(list2);
long result_part2 = 0;
foreach (var key in occ1.Keys)
{
    if (occ2.ContainsKey(key))
        result_part2 += key * occ1[key] * occ2[key];
}
Console.WriteLine(result_part2);

Dictionary<int, int> ComputeOccurrenceTable(IEnumerable<int> list)
{
    Dictionary<int, int> res = [];
    foreach (int value in list)
    {
        if (!res.ContainsKey(value))
            res.Add(value, 0);
        res[value]++;
    }
    return res;
}