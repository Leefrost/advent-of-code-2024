using AdventOfCode.Extensions;
using AdventOfCode.Structures;

namespace AdventOfCode.Days;

public static class Day18
{
    public static int Part1(string input)
    {
        var obstructions = File.ReadAllLines(input)
            .Select(line =>
            {
                var coords = line.Split(',');
                return (int.Parse(coords[0]), int.Parse(coords[1]));
            }).ToArray();

        var (map, start, end) = CreateMap(new Vector2(71, 71));
        CorruptMap(map, 1024, obstructions);

        var result = FindPath(map, start, end, map.CreateBuffer<int>());
        return result;
    }

    public static string Part2(string input)
    {
        var obstructions = File.ReadAllLines(input)
            .Select(line =>
            {
                var coords = line.Split(',');
                return (int.Parse(coords[0]), int.Parse(coords[1]));
            }).ToArray();

        var (map, start, end) = CreateMap(new Vector2(71, 71));
        var buffer = map.CreateBuffer<int>();

        var (head, tail) = (1024 + 1, obstructions.Length);
        while (tail - head > 1)
        {
            var mid = (head + tail) / 2;

            var mapCopy = map.Copy();
            CorruptMap(mapCopy, mid, obstructions);

            if (FindPath(mapCopy, start, end, buffer) == -1)
                tail = mid;
            else
                head = mid;
        }

        var (x, y) = obstructions[head];
        return $"{x},{y}";
    }

    private static int FindPath(Map2<char> map, int start, int end, int[] distances)
    {
        Array.Fill(distances, int.MaxValue);

        var queue = new PriorityQueue<int, int>();
        var visited = new HashSet<int>();

        queue.Enqueue(start, H(map, start, end));
        distances[start] = 0;

        while (queue.TryDequeue(out var current, out _))
        {
            if (current == end)
                return distances[current];

            if (map[current] == '#' || !visited.Add(current))
                continue;

            Array.ForEach(map.Directions, d =>
            {
                var next = current + d;
                var nextD = distances[current] + 1;

                if (distances[next] > nextD)
                {
                    distances[next] = nextD;
                    queue.Enqueue(next, nextD + H(map, next, end));
                }
            });
        }

        return -1;
    }

    private static int H(Map2<char> map, int from, int to)
    {
        var (cx, cy) = map.D1toD2(from);
        var (ex, ey) = map.D1toD2(to);

        return Math.Abs(cx - ex) + Math.Abs(cy - ey);
    }

    private static void CorruptMap(Map2<char> map, int steps, (int x, int y)[] obstructions)
        => obstructions.Take(steps).Each(p => map[p.x + 1, p.y + 1] = '#');

    private static (Map2<char> map, int start, int end) CreateMap(Vector2 size)
    {
        var map = new Map2<char>(new char[(size.X + 2) * (size.Y + 2)], size.X + 2);

        Array.Fill(map.Data, '.');
        map.FillBorders('#');

        var start = map.D2toD1(1, 1);
        var end = map.D2toD1(size.X, size.Y);

        return (map, start, end);
    }
}