using System.Text.RegularExpressions;
using AdventOfCode.Structures;

namespace AdventOfCode.Days;

public static class Day24
{
    private static Rule[] _rules = [];
    private static readonly Dictionary<string, bool> _values = [];

    private static readonly Regex ValueRegex = new(@"(?<name>.*?)\:\s+(?<value>\d)", RegexOptions.Compiled);

    private static readonly Regex OpRegex =
        new(@"(?<in1>\S*)\s+(?<op>\S*)\s+(?<in2>\S*)\s+->\s+(?<out>\S*)", RegexOptions.Compiled);

    public static string Part1(string input)
    {
        var lines = File.ReadAllText(input);

        _values.Clear();

        ValueRegex.Matches(lines)
            .Each(m => _values.Add(m.Groups["name"].Value,
                Convert.ToInt16(m.Groups["value"].Value) > 0));

        _rules = OpRegex.Matches(lines)
            .Select(m => new Rule
            (
                Enum.Parse<Op>(m.Groups["op"].Value),
                m.Groups["in1"].Value,
                m.Groups["in2"].Value,
                m.Groups["out"].Value
            )).ToArray();

        var result = Run([]).ToString();
        return result;
    }

    public static string Part2(string input)
    {
        var lines = File.ReadAllText(input);

        _values.Clear();

        ValueRegex.Matches(lines)
            .Each(m => _values.Add(m.Groups["name"].Value,
                Convert.ToInt16(m.Groups["value"].Value) > 0));

        _rules = OpRegex.Matches(lines)
            .Select(m => new Rule
            (
                Enum.Parse<Op>(m.Groups["op"].Value),
                m.Groups["in1"].Value,
                m.Groups["in2"].Value,
                m.Groups["out"].Value
            )).ToArray();

        var result = string.Join(",", GenerateSwaps([]).Keys.Distinct().Order());
        return result;
    }

    private static Dictionary<string, string> GenerateSwaps(Dictionary<string, string> swaps)
    {
        var count = Convert.ToInt32(_values.Keys
            .Where(k => k.StartsWith('x'))
            .Order()
            .Last()[1..]);

        var co = GetOut(swaps, "x00", "y00", Op.AND);
        for (var i = 1; i <= count; i++)
        {
            void Swap(string a, string b)
            {
                swaps.Add(a, b);
                swaps.Add(b, a);
                i--;
            }

            var x = $"x{i:d2}";
            var y = $"y{i:d2}";
            var z = $"z{i:d2}";

            var xor = GetOut(swaps, x, y, Op.XOR)!;
            var and = GetOut(swaps, x, y, Op.AND)!;

            var cXor = GetOut(swaps, co, xor, Op.XOR);
            var cAnd = GetOut(swaps, co, xor, Op.AND);

            if (cXor == null && cAnd == null)
            {
                Swap(xor, and);
                continue;
            }

            if (cXor != z)
            {
                Swap(cXor!, z);
                continue;
            }

            co = GetOut(swaps, and, cAnd, Op.OR);
        }
        return swaps;
    }

    private static string? GetOut(Dictionary<string, string> swaps, string? in1, string? in2, Op op)
    {
        return ApplySwaps(swaps, GetOut(in1, in2, op) ?? GetOut(in2, in1, op));
    }
    private static string? GetOut(string? in1, string? in2, Op op)
    {
        return _rules.Where(r => r.In1 == in1 && r.In2 == in2 && r.Op == op).FirstOrDefault()?.Out;
    }

    private static ulong Run(Dictionary<string, string> swaps)
    {
        var memory = new Dictionary<string, bool>(_values);

        for (var changed = true; changed;)
        {
            changed = false;

            foreach (var rule in _rules)
            {
                var @out = ApplySwaps(swaps, rule.Out)!;

                if (!memory.ContainsKey(@out) &&
                    memory.TryGetValue(rule.In1, out var in1) &&
                    memory.TryGetValue(rule.In2, out var in2))
                {
                    memory.Add(@out, Calculate(in1, in2, rule.Op));
                    changed = true;
                }
            }
        }

        return BitArray2UInt64(memory.Where(kv => kv.Key.StartsWith('z'))
            .OrderBy(kv => kv.Key)
            .Select(kv => kv.Value));
    }

    private static string? ApplySwaps(Dictionary<string, string> swaps, string? @out)
    {
        return @out is not null && swaps.TryGetValue(@out, out var result) ? result : @out;
    }

    private static bool Calculate(bool a, bool b, Op op)
    {
        return op switch
        {
            Op.AND => a & b,
            Op.OR => a | b,
            Op.XOR => a ^ b,

            _ => throw new NotImplementedException()
        };
    }

    private static ulong BitArray2UInt64(IEnumerable<bool> arr)
    {
        return arr.Select((d, i) => (d, i)).Where(b => b.d).Aggregate(0UL, (acc, b) => acc |= 1UL << b.i);
    }

    private static ulong GetNum(Dictionary<string, bool> memory, char num)
    {
        return BitArray2UInt64(memory.Where(kv => kv.Key.StartsWith(num))
            .OrderBy(kv => kv.Key)
            .Select(kv => kv.Value));
    }

    private enum Op { AND, OR, XOR }

    private record Rule(Op Op, string In1, string In2, string Out);
}