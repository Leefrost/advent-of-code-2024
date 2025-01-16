namespace AdventOfCode.Days;

public static class Day11
{
    private static Dictionary<(long number, int iterations), long> Cache { get; } = new();

    public static long Part1(string input)
    {
        var list = File.ReadAllLines(input)
            .SelectMany(line => line.Split(' ').Select(long.Parse).ToArray())
            .ToArray();

        return list.CountDescendants(25);
    }

    public static long Part2(string input)
    {
        var list = File.ReadAllLines(input)
            .SelectMany(line => line.Split(' ').Select(long.Parse).ToArray()).ToArray();

        return list.CountDescendants(75);
    }

    private static long CountDescendants(this IEnumerable<long> numbers, int iterations)
        => numbers.Sum(number => number.CountDescendants(iterations));

    private static long CountDescendants(this long number, int iterations)
        => Cache.TryGetValue((number, iterations), out var count)
            ? count
            : Cache[(number, iterations)] = number.FullCount(iterations);

    private static long FullCount(this long number, int iterations)
        => iterations == 0
            ? 1
            : number.Expand().CountDescendants(iterations - 1);

    private static long[] Expand(this long number)
        => number == 0
            ? [1]
            : number.DigitsCount() is var count && count % 2 == 0
                ? number.Split((count / 2).Power10())
                : [number * 2024];

    private static long[] Split(this long number, long divisor)
        => [number / divisor, number % divisor];

    private static long Power10(this int n)
        => n == 0
            ? 1
            : 10 * (n - 1).Power10();

    private static int DigitsCount(this long n)
        => n < 10
            ? 1
            : 1 + (n / 10).DigitsCount();
}