using AdventOfCode.Structures;

namespace AdventOfCode.Days;

public static class Day23
{
    private const int T = 't' - 'a';
    private static readonly Dictionary<int, HashSet<int>> _graph = [];

    public static int Part1(string input)
    {
        var lines = File.ReadAllLines(input)
            .Select(line => line.Split('-')
                .Select(t => t.Trim()).ToArray()
            ).ToArray();

        foreach (var (v1, v2) in lines.Select(t => (V2I(t[0]), V2I(t[1]))))
        {
            _graph.TryAdd(v1, []);
            _graph[v1].Add(v2);

            _graph.TryAdd(v2, []);
            _graph[v2].Add(v1);
        }

        HashSet<int> paths = [];

        _graph.Keys.Where(k => StartsWith(k, T))
            .Each(k => paths.UnionWith(FindSet(k)));

        return paths.Count;
    }

    public static string Part2(string input)
    {
        var lines = File.ReadAllLines(input)
            .Select(line => line.Split('-')
                .Select(t => t.Trim()).ToArray()
            ).ToArray();

        foreach (var (v1, v2) in lines.Select(t => (V2I(t[0]), V2I(t[1]))))
        {
            _graph.TryAdd(v1, []);
            _graph[v1].Add(v2);

            _graph.TryAdd(v2, []);
            _graph[v2].Add(v1);
        }

        var result = string.Join(",", Cliques(_graph)
            .MaxBy(c => c.Count).Select(I2V).OrderBy(x => x));
        return result;
    }

    private static HashSet<int> FindSet(int v)
    {
        HashSet<int> list = [];

        foreach (var lvl1 in _graph[v])
        foreach (var lvl2 in _graph[lvl1])
        foreach (var lvl3 in _graph[lvl2].Where(c => c == v))
            list.Add(ToSet(v, lvl1, lvl2));

        return list;
    }

    private static List<HashSet<int>> Cliques(Dictionary<int, HashSet<int>> graph)
    {
        var cliques = new List<HashSet<int>>();
        BronKerbosch([], [..graph.Keys], [], graph, cliques);

        return cliques;
    }

    private static void BronKerbosch(HashSet<int> r, HashSet<int> p, HashSet<int> x, Dictionary<int, HashSet<int>> graph, List<HashSet<int>> cliques)
    {
        if (p.Count == 0 && x.Count == 0)
        {
            cliques.Add(r);
            return;
        }

        foreach (var v in p.Except(graph[NonNeighborsVertex(p, x, graph)]))
        {
            BronKerbosch
            (
                [..r, v],
                [..p.Intersect(graph[v])],
                [..x.Intersect(graph[v])],
                graph,
                cliques
            );

            p.Remove(v);
            x.Add(v);
        }
    }

    private static int NonNeighborsVertex(HashSet<int> p, HashSet<int> x, Dictionary<int, HashSet<int>> graph)
    {
        return p.Union(x).OrderByDescending(v => graph[v].Count).First();
    }

    private static int ToSet(int v1, int v2, int v3)
    {
        Sort3(ref v1, ref v2, ref v3);
        return ((v1 << 10) + v2 << 10) + v3;
    }

    private static int V2I(string s)
    {
        return (s[0] - 'a' << 5) + (s[1] - 'a');
    }

    private static string I2V(int id)
    {
        return new string([(char)((id >> 5) + 'a'), (char)((id & 31) + 'a')]);
    }

    private static bool StartsWith(int v, int target)
    {
        return v >> 5 == target;
    }

    private static void Sort2(ref int a, ref int b)
    {
        if (a > b)
            (b, a) = (a, b);
    }

    private static void Sort3(ref int a, ref int b, ref int c)
    {
        Sort2(ref a, ref b);
        Sort2(ref a, ref c);
        Sort2(ref b, ref c);
    }
}