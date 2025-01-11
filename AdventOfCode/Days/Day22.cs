using System.Collections.Concurrent;

namespace AdventOfCode.Days;

public static class Day22
{
    private const long Mod = 16777216L;
    private const long SeqSize = 4;
    private const int Iterations = 2000;
    private const int Base10 = 10;

    private static long[] _numbers = [];

    public static string Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        _numbers = lines.Select(line => Convert.ToInt64(line)).ToArray();
        
        var result = _numbers.AsParallel()
            .Sum(s => CalculateSecret(s, Iterations))
            .ToString();
        
        return result;
    }

    public static string Part2(string input)
    {
        var lines = File.ReadAllLines(input);
        _numbers = lines.Select(long.Parse).ToArray();
        
        var sums = new ConcurrentDictionary<int, long>();
        _numbers
            .AsParallel()
            .ForAll(n => CalculateSeqs(n, Iterations, sums));  
        
        var result = sums.Values.AsParallel().Max().ToString();
        return result;
    }
    
    private static long CalculateSecret(long secret, int iterations)
    {
        for (int i = 0; i < iterations; i++)
            secret = NextSecret(secret);

        return secret;
    }
    
    private static void CalculateSeqs(long secret, long iterations, ConcurrentDictionary<int, long> sums)
    {
        var added = new HashSet<int>();
        iterations -= SeqSize;

        var seq = 0;                
        var prev = 0L;

        void update()
        {
            var digit = secret % Base10;
            seq = (seq << 8) + (byte)(digit - prev + Base10);
            secret = NextSecret(secret);
            prev = digit;
        }
        
        for (int i = 0; i < SeqSize; i++)        
            update();

        for (int i = 0; i < iterations; i++)
        {
            update();

            if (!added.Contains(seq))
            {
                sums.AddOrUpdate(seq, prev, (_, v) => v + prev);
                added.Add(seq);
            }
        }
    }
    
    private static long NextSecret(long secret)
    {
        var step1 = MixAndPrune(secret, secret << 6);
        var step2 = MixAndPrune(step1, step1 >> 5);
        return MixAndPrune(step2, step2 << 11);
    }

    private static long MixAndPrune(long secret, long num)
        => (secret ^ num) % Mod;

}