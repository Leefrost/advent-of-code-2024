using System.Text.RegularExpressions;

namespace AdventOfCode.Days;

public static class Day17
{
    private const int A = 0;
    private const int B = 1;
    private const int C = 2;
    private static readonly Regex NumRegex = new(@"\d+");
    
    private delegate void Instruction(long a, long b, ref long output);

    private enum Code
    {
        ADV = 0,
        BXL = 1,
        BST = 2,
        JNZ = 3,
        BXC = 4,
        OUT = 5,
        BDV = 6,
        CDV = 7
    }

    public static string Part1(string input)
    {
        const int programIndex = 4;
        const int numberOfRegisters = 3;

        var lines = File.ReadAllLines(input);

        var data = Enumerable.Range(0, numberOfRegisters)
            .Select(i => Convert.ToInt64(NumRegex.Match(lines[i]).Value))
            .ToArray();

        var program = NumRegex.Matches(lines[programIndex])
            .Select(m => Convert.ToByte(m.Value))
            .ToArray();

        var registers = new long[numberOfRegisters];
        Array.Copy(data, registers, data.Length);

        var result = string.Join(",", Run(registers, program));
        return result;
    }

    public static long Part2(string input)
    {
        const int programIndex = 4;
        var lines = File.ReadAllLines(input);

        var program = NumRegex.Matches(lines[programIndex])
            .Select(m => Convert.ToByte(m.Value))
            .ToArray();

        var a = 0L;
        while (true)
        {
            var output = Run([a, 0, 0], program);
            var match = program.TakeLast(output.Length)
                .Zip(output, (p, ot) => p == ot)
                .All(b => b);

            if (match && output.Length == program.Length)
                break;

            a = match ? a << 3 : a + 1;
        }

        return a;
    }

    private static byte[] Run(long[] registers, byte[] p)
    {
        List<byte> output = [];

        for (var opIdx = 0; opIdx < p.Length;)
            opIdx = RunOp(registers, p, output, opIdx, p[opIdx + 1], Combo(registers, p[opIdx + 1]));

        return [.. output];
    }

    private static int RunOp(long[] reg, byte[] p, List<byte> output, int opIdx, int literal, long combo)
    {
        return (Code)p[opIdx] switch
        {
            Code.ADV => Do(opIdx, reg[A], combo, ref reg[A], Dv),
            Code.BDV => Do(opIdx, reg[A], combo, ref reg[B], Dv),
            Code.CDV => Do(opIdx, reg[A], combo, ref reg[C], Dv),

            Code.BST => Do(opIdx, combo, 8, ref reg[B], Mod),

            Code.BXL => Do(opIdx, reg[B], literal, ref reg[B], Xor),
            Code.BXC => Do(opIdx, reg[B], reg[C], ref reg[B], Xor),

            Code.JNZ => Jmp(opIdx, reg[A], literal),
            Code.OUT => opIdx + Out(output, combo),

            _ => -1
        };
    }

    private static int Do(int opIdx, long a, long b, ref long output, Instruction instruction)
    {
        instruction(a, b, ref output);
        return opIdx + 2;
    }

    private static int Out(List<byte> output, long literal)
    {
        output.Add((byte)(literal % 8));
        return 2;
    }

    private static int Jmp(int op, long check, int literal)
    {
        return check == 0 ? op + 2 : literal;
    }

    private static void Mod(long a, long b, ref long output)
    {
        output = a % b;
    }

    private static void Xor(long a, long b, ref long output)
    {
        output = a ^ b;
    }

    private static void Dv(long a, long b, ref long output)
    {
        output = a / (1L << (int)b);
    }

    private static long Combo(long[] r, int n)
    {
        return n >= 4 ? r[n - 4] : n;
    }
}