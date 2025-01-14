using System.Text.RegularExpressions;

namespace AdventOfCode.Days;

public static class Day3
{
    private abstract record Instruction;

    private record Multiply(int A, int B) : Instruction;

    private record Pause : Instruction;

    private record Resume : Instruction;
    
    public static long Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        var instructions = lines.SelectMany(Parse).ToList();
        
        var result = instructions.OfType<Multiply>().SumProducts();
        return result;
    }
    
    public static long Part2(string input)
    {
        var lines = File.ReadAllLines(input);
        
        var instructions = lines.SelectMany(Parse).ToList();
        var result = instructions.SumProducts();
        return result;
    }

    private static int SumProducts(this IEnumerable<Instruction> instructions)
        => instructions.Aggregate(
            (sum: 0, include: true),
            (acc, instruction) => instruction switch
            {
                Pause => (acc.sum, include: false),
                Resume => (acc.sum, include: true),
                Multiply mul when acc.include => (sum: acc.sum + mul.A * mul.B, acc.include),
                _ => acc
            }).sum;

    private static IEnumerable<Instruction> Parse(this string text)
        => Regex.Matches(text, @"(?<mul>mul)\((?<a>\d+),(?<b>\d+)\)|(?<dont>don't)\(\)|(?<do>do)\(\)")
            .Select(match => match switch
            {
                _ when match.Groups["dont"].Success => new Pause(),
                _ when match.Groups["do"].Success => (Instruction)new Resume(),
                _ => new Multiply(int.Parse(match.Groups["a"].Value), int.Parse(match.Groups["b"].Value))
            });
}