string[] lines = File.ReadAllLines("input.txt");

// part1
int nbSafeReports = 0;
int nbSafeReports2 = 0;
foreach (var line in lines)
{
    var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var report = tokens.Select(x => Int32.Parse(x)).ToList();
    
    if (IsReportSafe(report))
    {
        nbSafeReports++;
        nbSafeReports2++;
    }
    else if (IsReportSafeWithDampener(report))
    {
        nbSafeReports2++;
    }
}

Console.WriteLine(nbSafeReports);
Console.WriteLine(nbSafeReports2);

static bool IsReportSafe(List<int> report)
{
    bool hasIncrease = false;
    bool hasDecrease = false;
    for (int i = 1; i < report.Count; i++)
    {
        if (report[i - 1] < report[i])
            hasIncrease = true;
        else if (report[i - 1] > report[i])
            hasDecrease = true;

        int gap = Math.Abs(report[i - 1] - report[i]);
        if (gap == 0 || gap > 3)
            return false;
    }
    return !(hasIncrease && hasDecrease);
}

static bool IsReportSafeWithDampener(List<int> report)
{
    if (IsReportSafe(report))
        return true;

    for (int i = 0; i < report.Count; ++i)
    {
        List<int> subreport = new(report);
        subreport.RemoveAt(i);
        if (IsReportSafe(subreport))
            return true;
    }

    return false;
}
