namespace AdventOfCode.Days;

public static class Day5
{
    public static int Part1(string input)
    {
        List<int[]> data = [];
        Dictionary<int, List<int>> rules = [];
        
        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line is not null && line != string.Empty && line.Contains('|'))
            {
                var values = line.Split('|').Select(int.Parse).ToArray();
                var (start, end) = (values.First(), values.Last());

                rules.TryAdd(start, []);
                rules.TryAdd(end, []);

                rules[start].Add(end);
            }

            if (line is not null && line != string.Empty && line.Contains(','))
            {
                var values = line.Split(',').Select(int.Parse).ToArray();
                data.Add(values);
            }
        }

        return data
            .Where(row => IsOrderCorrect(row, rules))
            .Sum(row => row[row.Length / 2]);
    }

    public static int Part2(string input)
    {
        List<int[]> data = [];
        Dictionary<int, List<int>> rules = [];
        var comparer = Comparer<int>.Create((start, end) => rules[end].Contains(start) ? 1 : -1);
        
        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line is not null && line != string.Empty && line.Contains('|'))
            {
                var values = line.Split('|').Select(int.Parse).ToArray();
                var (start, end) = (values.First(), values.Last());

                rules.TryAdd(start, []);
                rules.TryAdd(end, []);

                rules[start].Add(end);
            }

            if (line is not null && line != string.Empty && line.Contains(','))
            {
                var values = line.Split(',').Select(int.Parse).ToArray();
                data.Add(values);
            }
        }

        return data
            .Where(row => !IsOrderCorrect(row, rules))
            .Select(row => row.OrderBy(item => item, comparer).ToArray())
            .Sum(row => row[row.Length / 2]);
    }

    private static bool IsOrderCorrect(int[] row, Dictionary<int, List<int>> rules)
    {
        return row.Zip(row.Skip(1), (start, end) => !rules[end].Contains(start)).All(s => s);
    }
}