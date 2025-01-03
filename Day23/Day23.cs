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
            neighbours = []
        });
    graph[computer1].neighbours.Add(computer2);

    if (!graph.ContainsKey(computer2))
        graph.Add(computer2, new Node
        {
            name = computer2,
            neighbours = []
        });
    graph[computer2].neighbours.Add(computer1);
}

Console.WriteLine($"Part 1: {ComputePart1()}");

string password = ComputeLongestPassword();
Console.WriteLine($"Part 2: {password}");

int ComputePart1()
{
    // find cliques of 3 nodes, starting with "t" nodes
    List<List<string>> cliques = [];
    foreach (var node1 in graph.Keys)
    {
        if (node1.StartsWith('t'))
        {
            for (int i = 0; i < graph[node1].neighbours.Count; i++)
            {
                string node2 = graph[node1].neighbours[i];
                for (int j = i + 1; j < graph[node1].neighbours.Count; j++)
                {
                    string node3 = graph[node1].neighbours[j];
                    if (graph[node2].neighbours.Contains(node3))
                    {
                        cliques.Add([node1, node2, node3]);
                    }
                }
            }
        }
    }

    // remove permutations by counting distinct passwords
    HashSet<string> passwords = [];
    foreach (var clique in cliques)
    {
        passwords.Add(ToPassword(clique));
    }
    return passwords.Count();
}

string ComputeLongestPassword()
{
    HashSet<string> passwords = [];

    // generate all cliques of length 3
    foreach (var node1 in graph.Keys)
    {
        for (int i = 0; i < graph[node1].neighbours.Count; i++)
        {
            string node2 = graph[node1].neighbours[i];
            for (int j = i + 1; j < graph[node1].neighbours.Count; j++)
            {
                string node3 = graph[node1].neighbours[j];
                if (graph[node2].neighbours.Contains(node3))
                {
                    passwords.Add(ToPassword([node1, node2, node3]));
                }
            }
        }
    }

    // iteratively generate cliques of length n by expanding upon cliques of length n-1 
    do
    {
        HashSet<string> nextPasswords = [];
        foreach (var password in passwords)
        {
            List<string> clique = FromPassword(password);
            List<string> neighbours = [];
            foreach (var node in graph[clique[0]].neighbours)
                neighbours.Add(node);
            for (int i = 1; i < clique.Count; i++)
            {
                neighbours = neighbours.Intersect(graph[clique[i]].neighbours).ToList();
            }
            foreach (var neighbour in neighbours)
            {
                var expandedClique = new List<string>(clique)
                {
                    neighbour
                };
                nextPasswords.Add(ToPassword(expandedClique));
            }
        }
        if (nextPasswords.Count == 0)
            return passwords.First();
        passwords = nextPasswords;
    } while (true);
}

string ToPassword(List<string> clique)
{
    return string.Join(',', clique.Order());
}

List<string> FromPassword(string password)
{
    var tokens = password.Split(',');
    return tokens.ToList();
}

struct Node
{
    public string name;
    public List<string> neighbours;
}
