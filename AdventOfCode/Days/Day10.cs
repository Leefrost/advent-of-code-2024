namespace AdventOfCode.Days;

public static class Day10
{
    public static int Part1(string input)
    {
        var map = File.ReadAllLines(input)
            .Select(line => line.ToArray()).ToArray();

        return map.GetTrailHeads().Sum(start => start.Score(map));
    }

    public static int Part2(string input)
    {
        var map = File.ReadAllLines(input)
            .Select(line => line.ToArray()).ToArray();

        return map.GetTrailHeads().Sum(start => start.Rating(map));
    }

    private static int Rating(this (int row, int col) point, char[][] map) =>
        point.WalkOn(map).Where(tuple => map.At(tuple.point) == '9').Sum(valueTuple => valueTuple.count); 

    private static int Score(this (int row, int col) start, char[][] map) =>
        start.WalkOn(map).Count(point => map.At(point.point) == '9');

    private static IEnumerable<((int row, int col) point, int count)> WalkOn(this (int row, int col) start, char[][] map)
    {
        var pathCount = new Dictionary<(int row, int col), int> { [start] = 1};
        var queue = new Queue<(int row, int col)>();
        
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            
            yield return (current, pathCount[current]);

            foreach (var neighbour in map.GetNeighbours(current))
            {
                if (pathCount.ContainsKey(neighbour))
                {
                    pathCount[neighbour] += pathCount[current];
                }
                else
                {
                    pathCount[neighbour] = pathCount[current];
                    queue.Enqueue(neighbour);
                }
            }
        }
    }

    private static IEnumerable<(int row, int col)> GetNeighbours(this char[][] map, (int row, int col) point) =>
        new[]
        {
            (point.row - 1, point.col), (point.row + 1, point.col),
            (point.row, point.col - 1), (point.row, point.col + 1),
        }
        .Where(neighbour => map.IsUphill(point, neighbour));

    private static bool IsUphill(this char[][] map, (int row, int col) a, (int row, int col) b) =>
        map.IsOnMap(a) && map.IsOnMap(b) && map.At(b) == map.At(a) + 1;
    
    private static bool IsOnMap(this char[][] map, (int row, int col) point) =>
        point is { row: >= 0, col: >= 0 } && point.row < map.Length && point.col < map[point.row].Length;
    
    private static char At(this char[][] map, (int row, int col) point) =>
        map[point.row][point.col];

    private static IEnumerable<(int row, int col)> GetTrailHeads(this char[][] map) =>
        from row in Enumerable.Range(0, map.Length)
        from col in Enumerable.Range(0, map[0].Length)
        where map[row][col] == '0'
        select (row, col);
    
}