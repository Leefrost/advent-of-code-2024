using AdventOfCode.Structures;

namespace AdventOfCode.Days;

public static class Day20
{
    private const char Start = 'S';
    private const char End = 'E';

    private const char Border = '@';
    private const char Obstruction = '#';
    private const char Empty = '.';
    
    private class Bfs(Map2<char> map, HashSet<char> obstructions)
    {
        private readonly Queue<int> _queue = new(map.Data.Length);
        private readonly HashSet<int> _visited = new(map.Data.Length);

        public int[] Full(int start, int[] distances)
        {
            Array.Fill(distances, int.MaxValue);

            _queue.Clear();
            _visited.Clear();

            _queue.Enqueue(start);
            distances[start] = 0;

            while (_queue.TryDequeue(out var current))
            {
                if (obstructions.Contains(map[current]) || !_visited.Add(current))
                    continue;

                Array.ForEach(map.Directions, d =>
                {
                    var next = current + d;
                    var nextD = distances[current] + 1;

                    if (nextD < distances[next])
                    {
                        distances[next] = nextD;
                        _queue.Enqueue(next);
                    }
                });
            }

            return distances;
        }
    }

    public static int Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        var data = lines.SelectMany(line => line).ToArray();
        var col = lines.First().Length;

        var map = Map2<char>.WithBorders(data, col, Border);

        var result = Solve(map, 2, 100);
        return result;
    }

    public static int Part2(string input)
    {
        var lines = File.ReadAllLines(input);
        var data = lines.SelectMany(line => line).ToArray();
        var col = lines.First().Length;

        var map = Map2<char>.WithBorders(data, col, Border);

        var result = Solve(map, 20, 100);
        return result;
    }

    private static int Solve(Map2<char> map, int depth, int minDistance)
    {
        var bfs = new Bfs(map, [Obstruction, Border]);

        var start = Array.IndexOf(map.Data, Start);
        var end = Array.IndexOf(map.Data, End);

        var te = bfs.Full(start, map.CreateBuffer<int>());
        var ts = bfs.Full(end, map.CreateBuffer<int>());

        return map.Select((d, i) => (d, i))
            .Where(c => c.d != Border && c.d != Obstruction)
            .AsParallel()
            .Sum(cell => GetCheatCells(map, cell.i, depth, [])
                .Select(jmp => te[end] - (te[cell.i] + ts[jmp.loc] + jmp.dist))
                .Count(dst => dst >= minDistance));
    }

    private static IEnumerable<(int loc, int dist)> GetCheatCells(Map2<char> map, int loc, int depth,
        HashSet<int> visited)
    {
        var queue = new Queue<(int loc, int d)>();
        queue.Enqueue((loc, 0));

        while (queue.TryDequeue(out var data))
        {
            var (current, distance) = data;

            if (map[current] == Border || !visited.Add(current))
                continue;

            if ((map[current] == Empty || map[current] == End) && distance > 0)
                yield return (current, distance);

            if (distance >= depth)
                continue;

            Array.ForEach(map.Directions, d => queue.Enqueue((current + d, distance + 1)));
        }
    }
}