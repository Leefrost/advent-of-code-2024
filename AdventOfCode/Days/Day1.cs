namespace AdventOfCode.Days;

public static class Day1
{
    public static long Part1(string input)
    {
        List<int> left = [];
        List<int> right = [];

        var lines = File.ReadAllLines(input);
        foreach (var line in lines)
        {
            var values = line.Split("   ");

            left.Add(int.Parse(values[0]));
            right.Add(int.Parse(values[1]));
        }

        left.Sort();
        right.Sort();

        long sum = 0;
        for (var i = 0; i < left.Count; i++)
            sum += Math.Abs(left[i] - right[i]);

        return sum;
    }

    public static long Part2(string input)
    {
        List<int> left = [];
        var counter = new int[100000];

        var lines = File.ReadAllLines(input);
        foreach (var line in lines)
        {
            var values = line.Split("   ");

            var leftVal = int.Parse(values[0]);
            var rightVal = int.Parse(values[1]);

            left.Add(leftVal);
            counter[rightVal]++;
        }

        var result = left.Aggregate<int, long>(0, (current, t) => current + t * counter[t]);
        return result;
    }
}