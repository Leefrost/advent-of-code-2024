using AdventOfCode.Extensions;
using AdventOfCode.Structures;

namespace AdventOfCode.Days;

public static class Day21
{
    private const char Gap = ' ';
    private const char Enter = 'A';

    private static Map2<char> _keyboard = new([], 1);
    private static Map2<char> _robotKeyboard = new([], 1);

    private static readonly Dictionary<(string, int), ulong> Cache = [];
    private static readonly List<int> PathBuffer = new(64);
    private static readonly int[] DistanceBuffer = new int[64];

    public static string Part1(string input)
    {
        var lines = File.ReadAllLines(input);

        _keyboard = Map2<char>.WithBorders(['7', '8', '9', '4', '5', '6', '1', '2', '3', Gap, '0', Enter], 3, Gap);
        _robotKeyboard = Map2<char>.WithBorders([Gap, '^', Enter, '<', 'v', '>'], 3, Gap);

        var result = Solve(2, lines);
        return result;
    }

    public static string Part2(string input)
    {
        var lines = File.ReadAllLines(input);

        _keyboard = Map2<char>.WithBorders(['7', '8', '9', '4', '5', '6', '1', '2', '3', Gap, '0', Enter], 3, Gap);
        _robotKeyboard = Map2<char>.WithBorders([Gap, '^', Enter, '<', 'v', '>'], 3, Gap);

        var result = Solve(25, lines);
        return result;
    }

    private static string Solve(int depth, IEnumerable<string> instructions)
    {
        Cache.Clear();
        return instructions.BigSum(code => SolveNumeric(code, depth)).ToString();
    }

    private static ulong BigSum<T>(this IEnumerable<T> source, Func<T, ulong> func)
        => source.Aggregate(0UL, (acc, n) => acc + func(n));

    private static ulong SolveNumeric(string code, int depth)
        => Convert.ToUInt64(code[..^1]) * AssembleSequence(_keyboard, code)
            .BigSum(ins => SolveDirectional(ins, depth));

    private static ulong SolveDirectional(string code, int depth)
    {
        var tuple = (code, depth);

        if (Cache.TryGetValue(tuple, out var value))
            return value;

        if (depth == 0)
            return (ulong)code.Length;

        return Cache.AddAndReturn(tuple, AssembleSequence(_robotKeyboard, code)
            .BigSum(ins => SolveDirectional(ins, depth - 1)));
    }
    
    private static IEnumerable<string> AssembleSequence(Map2<char> keyboard, string code)
    {
        var location = Array.IndexOf(keyboard.Data, Enter);

        foreach (var symbol in code)
        {
            var target = Array.IndexOf(keyboard.Data, symbol);

            if (target == location)
            {
                yield return Enter.ToString();
                continue;
            }

            yield return GetInstructions(keyboard, location, target);
            location = target;
        }
    }

    private static string GetInstructions(Map2<char> keyboard, int start, int end)
    {
        var distances = Bfs(keyboard, start, end, DistanceBuffer);

        PathBuffer.Clear();
        ReconstructPath(keyboard, distances, end, start, PathBuffer);

        var instructions = PathBuffer.Reverse<int>()
            .Select(keyboard.InvDdx)
            .OrderBy(Ddx2W)
            .ToArray();

        if (!IsPossible(keyboard, start, instructions))
            instructions = [.. instructions.OrderByDescending(Ddx2W)];

        return new string(instructions.Select(Ddx2C).ToArray()) + Enter;
    }

    private static bool IsPossible(Map2<char> keyboard, int location, int[] path)
    {
        var possibility = true;

        for (var i = 0; possibility && i < path.Length; i++)
        {
            location = keyboard.Next(location, path[i]);
            possibility &= keyboard[location] != Gap;
        }

        return possibility;
    }

    private static void ReconstructPath(Map2<char> map, int[] distances, int start, int end, List<int> path)
    {
        if (start == end)
            return;

        var next = Enumerable.Range(0, map.Directions.Length)
            .Select(ddx => (ddx, loc: map.Next(start, ddx)))
            .OrderBy(n => distances[n.loc])
            .First();

        path.Add(next.ddx);
        ReconstructPath(map, distances, next.loc, end, path);
    }

    private static int[] Bfs(Map2<char> map, int start, int end, int[] distances)
    {
        Array.Fill(distances, int.MaxValue);

        var queue = new PriorityQueue<int, int>();
        var visited = new HashSet<int>();

        queue.Enqueue(start, 0);
        distances[start] = 0;

        while (queue.TryDequeue(out var current, out var distance))
        {
            if (current == end)
                return distances;

            if (map[current] == Gap || !visited.Add(current))
                continue;

            Enumerable.Range(0, map.Directions.Length).Each(ddx =>
            {
                var next = map.Next(current, ddx);
                var nextD = distances[current] + 1;

                if (nextD < distances[next] && map[next] != Gap)
                {
                    distances[next] = nextD;
                    queue.Enqueue(next, nextD);
                }
            });
        }

        return distances;
    }

    private static int Ddx2W(int ddx)
        => ddx switch
        {
            0 => 2,
            1 => 4,
            2 => 3,
            3 => 1,

            _ => -1
        };

    private static char Ddx2C(int dir)
    {
        return dir switch
        {
            0 => '^',
            1 => '>',
            2 => 'v',
            3 => '<',

            _ => '0'
        };
    }
}