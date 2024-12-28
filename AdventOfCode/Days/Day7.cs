using System.Numerics;

namespace AdventOfCode.Days;

public static class Day7
{
    public static long Part1(string input)
    {
        var bridges = File.ReadAllLines(input)
            .Select(line => new Bridge(line)).ToArray();

        List<Func<long, long, long>> operations = [(a, b) => a * b, (a, b) => a + b];

        var result = bridges
            .Where(bridge => bridge.IsSolved(operations))
            .Sum(e => e.Answer);

        return result;
    }

    public static long Part2(string input)
    {
        var bridges = File.ReadAllLines(input)
            .Select(line => new Bridge(line)).ToArray();

        List<Func<long, long, long>> operations =
        [
            (a, b) => a * b,
            (a, b) => a + b,
            (a, b) => long.Parse(a.ToString() + b)
        ];

        var result = bridges
            .Where(bridge => bridge.IsSolved(operations))
            .Sum(e => e.Answer);

        return result;
    }

    private static IEnumerable<List<T>> Reduce<T>(this List<T> list, List<Func<T, T, T>> operation)
    {
        foreach (var reduce in operation)
            yield return ((T[]) [reduce(list[0], list[1])])
                .Concat(list.Skip(2))
                .ToList();
    }

    private static bool Solve<T>(this List<T> list, T result, List<Func<T, T, T>> reductions) where T : INumber<T>
    {
        if (list.Count > 1)
            return list.Reduce(reductions).Any(bridge => bridge.Solve(result, reductions));

        return list[0] == result;
    }

    private class Bridge
    {
        public Bridge(string line)
        {
            var pair = line.Split(": ");
            Answer = long.Parse(pair[0]);
            Inputs = pair[1].Split(' ').Select(long.Parse).ToList();
        }

        private List<long> Inputs { get; }
        public long Answer { get; }

        public bool IsSolved(List<Func<long, long, long>> reductions)
        {
            return Inputs.Solve(Answer, reductions);
        }
    }
}