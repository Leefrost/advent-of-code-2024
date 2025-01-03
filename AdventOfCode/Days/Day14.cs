using System.Text.RegularExpressions;

namespace AdventOfCode.Days;

public static class Day14
{
    private static readonly Regex Parsing =
        new(@"p=(?<px>-?\d+),(?<py>-?\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)");

    public static long Part1(string input)
    {
        var lines = File.ReadAllText(input);

        var robots = Parsing.Matches(lines).Select(match =>
            new Robot(
                new Vector2(ToInt(match, "px"), ToInt(match, "py")),
                new Vector2(ToInt(match, "vx"), ToInt(match, "vy"))
            )).ToArray();

        var predictions = robots
            .Select(robot => robot.Position).ToArray();

        var size = new Vector2(101, 103);
        var blocks = new Block[]
        {
            new(Vector2.Zero, new Vector2(size.X / 2, size.Y / 2)),
            new(new Vector2(size.X / 2 + 1, 0), new Vector2(size.X, size.Y / 2)),
            new(new Vector2(0, size.Y / 2 + 1), new Vector2(size.X / 2, size.Y)),
            new(new Vector2(size.X / 2 + 1, size.Y / 2 + 1), new Vector2(size.X, size.Y))
        };

        Prediction(size, 100, robots, predictions);
        var result = blocks
            .Select(block => predictions
                .Count(p => p.X >= block.Start.X && p.X < block.End.X && p.Y >= block.Start.Y && p.Y < block.End.Y))
            .Aggregate(1, (acc, vect) => acc * vect);
        return result;
    }

    public static long Part2(string input)
    {
        var lines = File.ReadAllText(input);

        var robots = Parsing.Matches(lines).Select(match =>
            new Robot(
                new Vector2(ToInt(match, "px"), ToInt(match, "py")),
                new Vector2(ToInt(match, "vx"), ToInt(match, "vy"))
            )).ToArray();

        var predictions = robots
            .Select(robot => robot.Position).ToArray();

        var size = new Vector2(101, 103);

        var iterations = Math.Max(size.X, size.Y);
        var variances = Enumerable.Range(0, iterations)
            .Select(i => Variance(Prediction(size, i, robots, predictions)))
            .ToArray();

        var (vx, vy) = (variances.IndexOfMin(v => v.x), variances.IndexOfMin(v => v.y));
        var result = vx + Mod(ModInv(size.X, size.Y) * (vy - vx), size.Y) * size.X;
        return result;
    }

    private static int IndexOfMin<T, TV>(this T[] source, Func<T, TV> selector)
        where TV : IComparable<TV>
    {
        var min = 0;
        for (var i = 1; i < source.Length; i++)
            if (selector(source[i]).CompareTo(selector(source[min])) < 0)
                min = i;

        return min;
    }

    private static (double x, double y) Variance(Vector2[] points)
    {
        var n = points.Length;

        var ax = points.Average(p => p.X);
        var ay = points.Average(p => p.Y);

        var vx = points.Sum(p => (p.X - ax) * (p.X - ax)) / (n - 1);
        var vy = points.Sum(p => (p.Y - ay) * (p.Y - ay)) / (n - 1);

        return (vx, vy);
    }

    private static Vector2[] Prediction(Vector2 size, int iterations, Robot[] robots, Vector2[] buffer)
    {
        for (var i = 0; i < robots.Length; i++)
        {
            buffer[i].X = Mod(robots[i].Position.X + robots[i].Velocity.X * iterations, size.X);
            buffer[i].Y = Mod(robots[i].Position.Y + robots[i].Velocity.Y * iterations, size.Y);
        }

        return buffer;
    }

    private static int Mod(int n, int m)
    {
        return (n % m + m) % m;
    }

    private static int ModInv(int n, int m)
    {
        int t = 0, nt = 1;
        int r = m, nr = n;

        while (nr != 0)
        {
            var q = r / nr;
            (r, nr) = (nr, r - q * nr);
            (t, nt) = (nt, t - q * nt);
        }

        return Mod(t, m);
    }

    private static int ToInt(Match match, string group)
    {
        return Convert.ToInt32(match.Groups[group].Value);
    }

    private record struct Vector2(int X, int Y)
    {
        public static Vector2 Zero => new(0, 0);

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }
    }

    private record Robot(Vector2 Position, Vector2 Velocity);

    private record Block(Vector2 Start, Vector2 End);
}