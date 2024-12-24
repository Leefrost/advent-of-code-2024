using System.Text.RegularExpressions;

namespace AdventOfCode.Days;

public static class Day3
{
    public static long Part(string input)
    {
        var lines = new List<string>();
        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line != null)
                lines.Add(line);
        }

        var instructions = lines.SelectMany(Parse).ToList();
        var task1 = instructions.OfType<Multiply>().SumProducts();
        var task2 = instructions.SumProducts();
        //return task1;
        return task2;
    }

    private static int SumProducts(this IEnumerable<Instruction> instructions) =>
        instructions.Aggregate(
            (sum: 0, include: true),
            (acc, instruction) => instruction switch
            {
                Pause => (sum: acc.sum, include: false),
                Resume => (sum: acc.sum, include: true),
                Multiply mul when acc.include => (sum: acc.sum + mul.A * mul.B, include: acc.include),
                _ => acc
            }).sum;
    
    private static IEnumerable<Instruction> Parse(this string text) =>
        Regex.Matches(text, @"(?<mul>mul)\((?<a>\d+),(?<b>\d+)\)|(?<dont>don't)\(\)|(?<do>do)\(\)")
            .Select(match => match switch
            {
                _ when match.Groups["dont"].Success => (Instruction)new Pause(),
                _ when match.Groups["do"].Success => (Instruction)new Resume(),
                _ => new Multiply(int.Parse(match.Groups["a"].Value), int.Parse(match.Groups["b"].Value))
            });
}

abstract record Instruction;
record Multiply(int A, int B): Instruction;
record Pause:Instruction;
record Resume: Instruction;