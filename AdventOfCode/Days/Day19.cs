using System.Collections.Concurrent;

namespace AdventOfCode.Days;

public static class Day19
{
    public static string Part1(string input)
    {
        var tokens = GetTokens(input, ",", t => t);
        HashSet<string> towels = [.. tokens[0]];

        var patterns = tokens.Skip(2).Select(t => t.First()).ToArray();

        var cache = new ConcurrentDictionary<string, ulong>(towels.ToDictionary(t => t, _ => 1UL));

        var result = patterns
            .AsParallel()
            .Count(p => DesignCombinations(p, cache, towels) > 0UL).ToString();
        return result;
    }

    public static string Part2(string input)
    {
        var tokens = GetTokens(input, ",", t => t);
        HashSet<string> towels = [.. tokens[0]];
        var patterns = tokens.Skip(2).Select(t => t.First()).ToArray();

        var cache = new ConcurrentDictionary<string, ulong>();

        var result = patterns
            .BigSum(x => DesignCombinations(x, cache, towels))
            .ToString();
        return result;
    }

    private static T[][] GetTokens<T>(string input, string separator, Func<string, T> converter)
        => File.ReadAllLines(input).Select(line => line.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => converter(t.Trim()))
                .ToArray())
            .ToArray();

    private static ulong DesignCombinations(string pattern, ConcurrentDictionary<string, ulong> cache,
        HashSet<string> towels)
        => cache.TryGetValue(pattern, out var value)
            ? value
            : cache.GetOrAdd(pattern,
                towels.Where(pattern.StartsWith).BigSum(t => DesignCombinations(pattern[t.Length..], cache, towels)));

    private static ulong BigSum<T>(this IEnumerable<T> source, Func<T, ulong> func)
        => source.Aggregate(0UL, (acc, n) => acc + func(n));
}