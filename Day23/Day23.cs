var input = File.ReadAllLines("input23.txt");

Dictionary<string, Node> graph = [];
foreach (var line in input)
{
    var tokens = line.Split('-');
    string computer1 = tokens[0];
    string computer2 = tokens[1];

    if (!graph.ContainsKey(computer1))
        graph.Add(computer1, new Node
        {
            name = computer1,
            neihgbours = []
        });
    graph[computer1].neihgbours.Add(computer2);

    if (!graph.ContainsKey(computer2))
        graph.Add(computer2, new Node
        {
            name = computer2,
            neihgbours = []
        });
    graph[computer2].neihgbours.Add(computer1);
}

// find cliques, starting with "t" nodes
List<Clique> cliques = [];
foreach (var node1 in graph.Keys)
{
    if (node1.StartsWith('t'))
    {
        for (int i = 0; i < graph[node1].neihgbours.Count; i++)
        {
            string node2 = graph[node1].neihgbours[i];
            for (int j = i + 1; j < graph[node1].neihgbours.Count; j++)
            {
                string node3 = graph[node1].neihgbours[j];
                if (graph[node2].neihgbours.Contains(node3))
                {
                    cliques.Add(new Clique
                    {
                        nodes = [node1, node2, node3]
                    });
                }
            }
        }
    }
}

List<string> orderedCliques = [];
foreach (var clique in cliques)
{
    orderedCliques.Add(string.Join('-', clique.nodes.OrderBy(x => x)));
}

Console.WriteLine($"Part 1: {orderedCliques.Distinct().Count()}");

struct Node
{
    public string name;
    public List<string> neihgbours;
}

struct Clique
{
    public List<string> nodes;
}