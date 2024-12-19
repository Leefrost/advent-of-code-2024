namespace AdventOfCode.Days;

public static class Day1
{
    public static long Part1(string input)
    {
        List<int> left = [];
        List<int> right = [];

        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split("   ");
    
            left.Add(int.Parse(values[0]));
            right.Add(int.Parse(values[1]));
        }

        left.Sort();
        right.Sort();

        long sum = 0;
        for (int i = 0; i < left.Count; i++)
        {
            sum += Math.Abs(left[i] - right[i]);
        }

        return sum;
    }

    public static long Part2(string input)
    {
        List<int> left = [];
        var counter = new int[100000];

        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split("   ");
            
            var leftVal = int.Parse(values[0]);
            var rightVal = int.Parse(values[1]);
            
            left.Add(leftVal);
            counter[rightVal]++;
        }

        long sum = 0;
        foreach (var t in left)
        {
            sum += t * counter[t];
        }

        return sum;
    }
}