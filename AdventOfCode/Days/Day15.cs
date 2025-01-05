using AdventOfCode.Structures;

namespace AdventOfCode.Days;

public static class Day15
{
    private const char Border = '#';
    private const char Robot = '@';
    private const char Box = 'O';
    private const char Empty = '.';

    private const char BoxLeft = '[';
    private const char BoxRight = ']';

    public static int Part1(string input)
    {
        var lines = File.ReadLines(input).ToList();
        
        var sx = lines.First().Length;
        var map = new Map2<char>(
            data:
            [
                ..lines
                    .TakeWhile(line => line.Length > 0 && line.First() == Border)
                    .SelectMany(line => line)
            ],
            columns: sx
        );

        var path = lines.Skip(map.Rows)
            .SkipWhile(line => line.Length == 0)
            .SelectMany(line => line.Select(Ch2D))
            .ToArray();
        
        var result = SumOfGps(map.Copy(), Box, path);
        return result;
    }

    public static int Part2(string input)
    {
        var lines = File.ReadLines(input).ToList();
        
        var sx = lines.First().Length;
        var map = new Map2<char>(
            data:
            [
                ..lines
                    .TakeWhile(line => line.Length > 0 && line.First() == Border)
                    .SelectMany(line => line)
            ],
            columns: sx
        );

        var path = lines.Skip(map.Rows)
            .SkipWhile(line => line.Length == 0)
            .SelectMany(line => line.Select(Ch2D))
            .ToArray();
        
        var wideMap = new Map2<char>(map.SelectMany(WideChar).ToArray(), 2 * map.Columns);
        var result = SumOfGps(wideMap.Copy(), BoxLeft, path);
        return result;
    }

    private static int SumOfGps(Map2<char> map, char target, int[] path)
    {
        var currentLocation = Array.IndexOf(map.Data, Robot);
        path.Each(d => currentLocation = TryMove(map, currentLocation, d));

        return map.Select((d, i) => (d, i))
            .Where(c => c.d == target)
            .Sum(c => Distance(map, c.i));
    }
    
    private static void Each<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    private static int TryMove(Map2<char> map, int location, int ddx)
    {
        var next = map.Next(location, ddx);
        if (map[location] == Border || !CanMove(map, location, ddx, [])) return location;

        Move(map, location, ddx, []);
        return next;
    }

    private static bool CanMove(Map2<char> map, int location, int ddx, HashSet<int> visited)
    {
        if (!visited.Add(location))
            return true;

        var next = map.Next(location, ddx);
        return map[location] switch
        {
            Border => false,
            Empty => true,
            BoxLeft => CanMove(map, next, ddx, visited) &&
                    CanMove(map, map.Next(location, Map2<char>.RIGHT), ddx, visited),
            BoxRight => CanMove(map, next, ddx, visited) && CanMove(map, map.Next(location, Map2<char>.LEFT), ddx, visited),
            _ => CanMove(map, next, ddx, visited)
        };
    }

    private static void Move(Map2<char> map, int location, int ddx, HashSet<int> visited)
    {
        if (!visited.Add(location))
            return;

        var next = map.Next(location, ddx);
        switch (map[location])
        {
            case Box:
            case Robot:
                Move(map, next, ddx, visited);
                break;
            case BoxLeft:
                Move(map, next, ddx, visited);
                Move(map, map.Next(location, Map2<char>.RIGHT), ddx, visited);
                break;
            case BoxRight:
                Move(map, next, ddx, visited);
                Move(map, map.Next(location, Map2<char>.LEFT), ddx, visited);
                break;
            default:
                return;
        }

        (map[next], map[location]) = (map[location], map[next]);
    }
    
    private static IEnumerable<char> WideChar(char symbol)
    {
        return symbol switch
        {
            Border => [Border, Border],
            Box => [BoxLeft, BoxRight],
            Empty => [Empty, Empty],
            Robot => [Robot, Empty],
            _ => throw new NotImplementedException()
        };
    }

    private static int Ch2D(char symbol)
    {
        return symbol switch
        {
            '^' => 0,
            '>' => 1,
            'v' => 2,
            '<' => 3,
            _ => -1
        };
    }

    private static int Distance(Map2<char> map, int location)
    {
        var (x, y) = map.D1toD2(location);
        return y * 100 + x;
    }
}