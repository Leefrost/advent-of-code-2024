using System.Text.RegularExpressions;

namespace AdventOfCode.Days;

public static class Day13
{
    private record Button(long X, long Y);

    private record Machine(Button A, Button B, long X, long Y)
    {
        internal long Solve(long offset = 0)
        {
            decimal ax = A.X;
            decimal ay = A.Y;
            decimal bx = B.X;
            decimal by = B.Y;

            var n = ((offset + X) * ay - (offset + Y) * ax) / (bx * ay - ax * by);
            var m = (offset + Y - n * by) / ay;

            if (n == (long)n && m == (long)m)
                return 3 * (long)m + (long)n;

            return 0;
        }

        internal static Machine Parse(string input)
        {
            var numbers = input.ReplaceLineEndings()
                .Replace(Environment.NewLine, "")
                .ParseDigits();

            return new Machine(
                new Button(numbers[0], numbers[1]),
                new Button(numbers[2], numbers[3]),
                numbers[4], numbers[5]);
        }
    }
    
    public static long Part1(string input)
    {
        var list = File.ReadAllText(input)
            .Split(Environment.NewLine + Environment.NewLine)
            .Select(Machine.Parse).ToList();

        var result = list
            .Select(m => m.Solve())
            .Where(cost => cost > 0)
            .Sum();

        return result;
    }

    public static long Part2(string input)
    {
        const long ten = 10_000_000_000_000;

        var list = File.ReadAllText(input)
            .Split(Environment.NewLine + Environment.NewLine)
            .Select(Machine.Parse).ToList();

        var result = list
            .Select(m => m.Solve(ten))
            .Where(cost => cost > 0)
            .Sum();

        return result;
    }

    private static long[] ParseDigits(this string input)
        => Regex.Matches(input, "[0-9]+", RegexOptions.Singleline)
            .Select(match => long.Parse(match.Value)).ToArray();
}