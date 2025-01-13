using AdventOfCode.Structures;

namespace AdventOfCode.Days;

public static class Day16
{
    private const char Border = '#';
    private const char Start = 'S';
    private const char End = 'E';

    private const int Forward = 1;
    private const int Rotate = 1000;

    public static int Part1(string input)
    {
        var lines = File.ReadLines(input)
            .Select(line => line).ToList();

        var map = new Map2<char>([.. lines.SelectMany(line => line)], lines.First().Length);
        var buffer = new int[map.Data.Length * map.Directions.Length];

        var end = Array.IndexOf(map.Data, End);
        DistanceToEnd(map, Array.IndexOf(map.Data, Start), end, buffer);

        var result = GetMinDistance(map, buffer, end);
        return result;
    }

    public static int Part2(string input)
    {
        var lines = File.ReadLines(input)
            .Select(line => line).ToList();

        var map = new Map2<char>([.. lines.SelectMany(line => line)], lines.First().Length);
        var buffer = new int[map.Data.Length * map.Directions.Length];

        var start = Array.IndexOf(map.Data, Start);
        var end = Array.IndexOf(map.Data, End);

        DistanceToEnd(map, Array.IndexOf(map.Data, Start), end, buffer);

        var nodes = new HashSet<int>();
        var lds = GetMinIndices(map, buffer, end);
        lds.Each(ld => VisitPlaces(map, buffer, ld, map.L2LD(start, Map2<char>.RIGHT), nodes));

        nodes.Add(map.L2LD(start, Map2<char>.RIGHT));
        var result = nodes.Select(ld => map.LD2L(ld).location)
            .Distinct()
            .Count();
        return result;
    }

    private static void VisitPlaces(Map2<char> map, int[] distances, int current, int end, HashSet<int> visited)
    {
        if (visited.Contains(current) | current == end)
            return;

        visited.Add(current);
        var (location, direction) = map.LD2L(current);

        var forward = new[]
        {
            Mod(direction + 2, map.Directions.Length)
        };

        var rotations = new[]
        {
            Mod(direction + 1, map.Directions.Length),
            Mod(direction + 3, map.Directions.Length)
        };

        rotations.Select(d => map.L2LD(location, d))
            .Concat(forward.Select(d => map.L2LD(map.Next(location, d),
                Mod(d + 2, map.Directions.Length))))
            .Where(p => distances[current] - distances[p] == Forward || distances[current] - distances[p] == Rotate)
            .Each(n => VisitPlaces(map, distances, n, end, visited));
    }

    private static void DistanceToEnd(Map2<char> map, int start, int end, int[] distances)
    {
        Array.Fill(distances, int.MaxValue);

        var visited = new HashSet<int>();
        var pq = new PriorityQueue<Node, int>();

        pq.Enqueue(new Node(start, 1, 0), 0);
        distances[start] = 0;

        while (pq.Count > 0)
        {
            var (loc, dir, distance) = pq.Dequeue();
            var ld = map.L2LD(loc, dir);

            if (visited.Contains(ld) || map[loc] == Border)
                continue;

            distances[ld] = distance;
            visited.Add(ld);

            if (loc == end)
                continue;

            pq.Enqueue(new Node(map.Next(loc, dir), dir, distance + Forward), distance + Forward);

            var nd = Mod(dir + 1, map.Directions.Length);
            var pd = Mod(dir + 3, map.Directions.Length);

            pq.Enqueue(new Node(loc, nd, distance + Rotate), distance + Rotate);
            pq.Enqueue(new Node(loc, pd, distance + Rotate), distance + Rotate);
        }
    }

    private static int GetMinDistance(Map2<char> map, int[] distances, int end)
    {
        var endUp = map.L2LD(end, Map2<char>.UP);
        return Enumerable
            .Range(0, map.Directions.Length)
            .Min(ld => distances[endUp + ld]);
    }

    private static int[] GetMinIndices(Map2<char> map, int[] distances, int end)
    {
        var endUp = map.L2LD(end, Map2<char>.UP);
        var directions = Enumerable
            .Range(0, map.Directions.Length)
            .ToArray();

        var min = directions.Min(ld => distances[endUp + ld]);
        return directions
            .Where(ld => distances[endUp + ld] == min)
            .Select(ld => ld + endUp)
            .ToArray();
    }



    private static int Mod(int n, int m)
    {
        return (n % m + m) % m;
    }

    private record Node(int Location, int Direction, int Distance);
}