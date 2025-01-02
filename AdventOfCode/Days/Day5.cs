namespace AdventOfCode.Days;

public static class Day5
{
    private static readonly Dictionary<int, List<int>> Rules = [];
    private static readonly List<int[]> Input = [];

    private static readonly Comparer<int>? TopComparer =
        Comparer<int>.Create((start, end) => Rules[end].Contains(start) ? 1 : -1);

    public static int Part1(string input)
    {
        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line is not null && line != string.Empty && line.Contains('|'))
            {
                var values = line.Split('|').Select(int.Parse).ToArray();
                var (start, end) = (values.First(), values.Last());

                Rules.TryAdd(start, []);
                Rules.TryAdd(end, []);

                Rules[start].Add(end);
            }

            if (line is not null && line != string.Empty && line.Contains(','))
            {
                var values = line.Split(',').Select(int.Parse).ToArray();
                Input.Add(values);
            }
        }

        return Input
            .Where(IsOrderCorrect)
            .Sum(row => row[row.Length / 2]);
    }

    public static int Part2(string input)
    {
        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line is not null && line != string.Empty && line.Contains('|'))
            {
                var values = line.Split('|').Select(int.Parse).ToArray();
                var (start, end) = (values.First(), values.Last());

                Rules.TryAdd(start, []);
                Rules.TryAdd(end, []);

                Rules[start].Add(end);
            }

            if (line is not null && line != string.Empty && line.Contains(','))
            {
                var values = line.Split(',').Select(int.Parse).ToArray();
                Input.Add(values);
            }
        }

        return Input
            .Where(row => !IsOrderCorrect(row))
            .Select(row => row.OrderBy(item => item, TopComparer).ToArray())
            .Sum(row => row[row.Length / 2]);
    }

    private static bool IsOrderCorrect(int[] row)
    {
        return row.Zip(row.Skip(1), (start, end) => !Rules[end].Contains(start)).All(s => s);
    }
}