using AdventOfCode.Extensions;
using AdventOfCode.Structures;

namespace AdventOfCode.Days;

public static class Day25
{

    private const int WIDTH = 5;
    private const char FILL = '#';

    private static int[][] _locks = [];
    private static int[][] _keys = [];

    public static int Part1(string input)
    {
        var lines = File.ReadAllLines(input);

        List<int[]> keys = [];
        List<int[]> locks = [];

        var temp = new int[WIDTH];
        bool? isKey = null;

        for (var i = 0; i <= lines.Length; i++)
        {
            if (i == lines.Length || string.IsNullOrWhiteSpace(lines[i]))
            {
                if (isKey!.Value)
                {
                    keys.Add([.. temp]);
                }
                else
                {
                    locks.Add([.. temp]);
                }

                Array.Fill(temp, 0);
                isKey = null;

                continue;
            }

            lines[i].Select(c => c == FILL ? 1 : 0)
                .Select((d, i) => (d, i))
                .Each(d => temp[d.i] += d.d);

            isKey ??= temp.Sum() > 0;
        }

        _keys = [.. keys];
        _locks = [.. locks];

        var result = _keys.AsParallel().Sum(k => _locks.Count(lk => IsFit(k, lk)));
        return result;
    }

    public static int Part2(string input)
    {
        return 0;
    }

    private static bool IsFit(int[] a, int[] b)
    {
        return a.Zip(b, (ai, bi) => ai + bi).All(d => d <= 7);
    }
}