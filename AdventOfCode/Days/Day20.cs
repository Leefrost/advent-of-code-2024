using AdventOfCode.Structures;

namespace AdventOfCode.Days;

public static class Day20
{
    private const char START = 'S';
    private const char END = 'E';

    private const char BORDER = '@';
    private const char OBSTRUCTION = '#';
    private const char EMPTY = '.';
    
    public static int Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        var data = lines.SelectMany(line => line).ToArray();
        var col = lines.First().Length;
        
        var map = Map2<char>.WithBorders(data, col, BORDER);
        
        var result = Solve(map, 2, 100);
        return result;
    }

    public static int Part2(string input)
    {
        var lines = File.ReadAllLines(input);
        var data = lines.SelectMany(line => line).ToArray();
        var col = lines.First().Length;
        
        var map = Map2<char>.WithBorders(data, col, BORDER);
        
        var result = Solve(map, 20, 100);
        return result;
    }
    
    private static int Solve(Map2<char> map, int depth, int minDistance)
    {
        var bfs = new BFS<char>(map, [OBSTRUCTION, BORDER]);

        var start = Array.IndexOf(map.Data, START);
        var end = Array.IndexOf(map.Data, END);

        var te = bfs.Full(start, map.CreateBuffer<int>());
        var ts = bfs.Full(end, map.CreateBuffer<int>());

        return map.Select((d, i) => (d, i))
            .Where(c => c.d != BORDER && c.d != OBSTRUCTION)
            .AsParallel()
            .Sum(cell => GetCheatCells(map, cell.i, depth, [])
                .Select(jmp => te[end] - (te[cell.i] + ts[jmp.loc] + jmp.dist))
                .Count(dst => dst >= minDistance));
    }
    
    private static IEnumerable<(int loc, int dist)> GetCheatCells(Map2<char> map, int loc, int depth, HashSet<int> visited)
    {
        var queue = new Queue<(int loc, int d)>();
        queue.Enqueue((loc, 0));

        while (queue.TryDequeue(out var data))
        {
            var (current, distance) = data;

            if (map[current] == BORDER || visited.Contains(current))
                continue;

            visited.Add(current);

            if ((map[current] == EMPTY || map[current] == END) && distance > 0)
                yield return (current, distance);

            if (distance >= depth)
                continue;

            Array.ForEach(map.Directions, d => queue.Enqueue((current + d, distance + 1)));
        }
    }

    private class BFS<T>(Map2<T> map, HashSet<T> obstructions)
    {
        private readonly Queue<int> queue = new(map.Data.Length);
        private readonly HashSet<int> visited = new(map.Data.Length);

        public int[] Full(int start, int[] distances)
        {
            Array.Fill(distances, int.MaxValue);

            queue.Clear();
            visited.Clear();

            queue.Enqueue(start);
            distances[start] = 0;

            while (queue.TryDequeue(out var current))
            {
                if (obstructions.Contains(map[current]) || visited.Contains(current))
                    continue;

                visited.Add(current);
                Array.ForEach(map.Directions, d =>
                {
                    var next = current + d;
                    var nextD = distances[current] + 1;

                    if (nextD < distances[next])
                    {
                        distances[next] = nextD;
                        queue.Enqueue(next);
                    }
                });
            }

            return distances;
        }
    }
}