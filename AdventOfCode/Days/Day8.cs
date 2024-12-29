namespace AdventOfCode.Days;

public record Point(int X, int Y)
{
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }

    public static Point operator *(Point a, int b)
    {
        return new Point(a.X * b, a.Y * b);
    }
}

public class Day8
{
    public static int Part1(string input)
    {
        var lines = File.ReadAllLines(input)
            .Select(line => line.ToArray()).ToArray();

        var x = lines.Length;
        var y = lines[0].Length;
        var antennas = new Dictionary<char, List<Point>>();

        lines
            .Select((row, index) => (row, index))
            .ToList()
            .ForEach(indexedRow => indexedRow.row.Select((letter, index) => (letter, index))
                .ToList()
                .Where(indexedLetter => indexedLetter.letter != '.')
                .ToList()
                .ForEach(ch =>
                {
                    antennas.TryAdd(ch.letter, []);
                    antennas[ch.letter].Add(new Point(ch.index, indexedRow.index));
                }));

        var count = Count(antennas, x, y, false);
        return count;
    }

    public static int Part2(string input)
    {
        var lines = File.ReadAllLines(input)
            .Select(line => line.ToArray()).ToArray();

        var x = lines.Length;
        var y = lines[0].Length;
        var antennas = new Dictionary<char, List<Point>>();

        lines
            .Select((row, index) => (row, index))
            .ToList()
            .ForEach(indexedRow => indexedRow.row.Select((letter, index) => (letter, index))
                .ToList()
                .Where(indexedLetter => indexedLetter.letter != '.')
                .ToList()
                .ForEach(ch =>
                {
                    antennas.TryAdd(ch.letter, []);
                    antennas[ch.letter].Add(new Point(ch.index, indexedRow.index));
                }));

        var count = Count(antennas, x, y);
        return count;
    }

    private static int Count(Dictionary<char, List<Point>> antennas, int x, int y, bool infinite = true)
    {
        var nodes = new HashSet<Point>();

        foreach (var positions in antennas.Values)
            for (var i = 0; i < positions.Count; i++)
            for (var k = i + 1; k < positions.Count; k++)
            {
                var d = positions[i] - positions[k];
                for (var (added, t) = (true, infinite ? 0 : 1); added; t++)
                {
                    var a1 = AddNode(nodes, positions[i] + d * t, x, y);
                    var a2 = AddNode(nodes, positions[k] - d * t, x, y);
                    added = infinite && a1 | a2;
                }
            }

        return nodes.Count;
    }

    private static bool AddNode(HashSet<Point> hashSet, Point point, int x, int y)
    {
        var canAdd = point is { Y: >= 0, X: >= 0 } && point.Y < y && point.X < x;
        _ = canAdd && hashSet.Add(point);

        return canAdd;
    }
}