namespace AdventOfCode.Days;

public static class Day2
{
    public static int Part1(string input)
    {
        var sum = 0;
        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(" ").Select(int.Parse).ToArray();

            if (IsSafe(values))
                sum++;
        }

        return sum;
    }

    private static bool IsSafe(int[] values)
    {
        var direction = values[1] - values[0] > 0;
        for (var i = 1; i < values.Length; i++)
        {
            if (Math.Abs(values[i] - values[i - 1]) > 3)
                return false;

            if (values[i] == values[i - 1])
                return false;

            var flow = values[i] - values[i - 1] > 0;
            if (flow != direction)
                return false;
        }

        return true;
    }

    public static int Part2(string input)
    {
        var list = new List<List<int>>();
        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line!.Split(" ").Select(int.Parse).ToList();
            list.Add(values);
        }

        var total = list.Count(items => items.Expand().Any(IsSafe));
        return total;
    }

    private static IEnumerable<List<int>> Expand(this List<int> values) => 
        new[] { values }.Concat(Enumerable.Range(0, values.Count).Select(values.ExceptAt));

    private static List<int> ExceptAt(this List<int> values, int index) => 
        values.Take(index).Concat(values.Skip(index + 1)).ToList();

    private static bool IsSafe(List<int> values) => 
        values.Count < 2 || values.IsSafe(Math.Sign(values[1] - values[0]));

    private static bool IsSafe(this List<int> values, int diffSign) => 
        values.Pairs().All(pair => pair.IsSave(diffSign));

    private static IEnumerable<(int prev, int next)> Pairs(this IEnumerable<int> pairs)
    {
        using var enumerator = pairs.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;

        var prev = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (prev, enumerator.Current);
            prev = enumerator.Current;
        }
    }

    private static bool IsSave(this (int prev, int next) pair, int diffSign) =>
        Math.Abs(pair.next - pair.prev) >= 1 &&
        Math.Abs(pair.next - pair.prev) <= 3 &&
        Math.Sign(pair.next - pair.prev) == diffSign;
}