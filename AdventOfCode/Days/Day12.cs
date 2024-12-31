using System.Runtime.InteropServices;

namespace AdventOfCode.Days;

public static class Day12
{
    public static int Part1(string input)
    {
        var grid = File.ReadAllLines(input)
            .Select(line => line.ToArray())
            .ToArray();

        var perimeterMap = grid.MapPerimeters();
        var regions = grid.MapRegions();

        var areas = regions.SelectMany(line => line).GroupBy(cell => cell)
            .ToDictionary(group => group.Key, group => group.Count());

        Dictionary<int, int> regionsToPerimeters = [];

        for (var row = 0; row < regions.Length; row++)
        for (var col = 0; col < regions[row].Length; col++)
        {
            var region = regions[row][col];
            CollectionsMarshal.GetValueRefOrAddDefault(regionsToPerimeters, region, out _) += perimeterMap[row][col];
        }

        var result = regionsToPerimeters.Sum(pair => pair.Value * areas[pair.Key]);
        return result;
    }

    public static int Part2(string input)
    {
        var grid = File.ReadAllLines(input)
            .Select(line => line.ToArray())
            .ToArray();

        var perimeterMap = grid.MapPerimeters();
        var regions = grid.MapRegions();

        var areas = regions.SelectMany(line => line).GroupBy(cell => cell)
            .ToDictionary(group => group.Key, group => group.Count());

        Dictionary<int, int> regionsToPerimeters = [];

        for (var row = 0; row < regions.Length; row++)
        for (var col = 0; col < regions[row].Length; col++)
        {
            var region = regions[row][col];
            CollectionsMarshal.GetValueRefOrAddDefault(regionsToPerimeters, region, out _) += perimeterMap[row][col];
        }

        Dictionary<int, int> corners = [];
        for (var row = 0; row < regions.Length; row++)
        for (var col = 0; col < regions[0].Length; col++)
        {
            var region = regions[row][col];
            corners.TryAdd(region, 0);
            if ("┏┛".Contains(regions.TopLeft(row, col))) corners[region]++;
            if ("┗┓".Contains(regions.TopRight(row, col))) corners[region]++;
            if ("┏┛".Contains(regions.BottomRight(row, col))) corners[region]++;
            if ("┗┓".Contains(regions.BottomLeft(row, col))) corners[region]++;
        }

        var result = corners.Sum(pair => pair.Value * areas[pair.Key]);
        return result;
    }

    private static int[][] MapRegions(this char[][] grid)
    {
        var result = new int[grid.Length][];
        for (var i = 0; i < result.Length; i++) result[i] = new int[grid[0].Length];
        var regionId = 1;
        for (var r = 0; r < grid.Length; r++)
        for (var c = 0; c < grid[r].Length; c++)
        {
            if (result[r][c] != 0) continue;
            result[r][c] = regionId;
            CheckNeighbours(grid[r][c], regionId, grid, result, r, c);
            regionId++;
        }

        return result;
    }

    private static char TopLeft(this int[][] grid, int row, int col)
    {
        var nw = grid.At(row - 1, col - 1) == grid.At(row, col);
        var w = grid.At(row, col - 1) == grid.At(row, col);
        var n = grid.At(row - 1, col) == grid.At(row, col);
        if (w && n && !nw) return '┛';
        if (w && !n) return grid.Top(row, col);
        if (n && !w) return grid.Left(row, col);
        if (w && n && nw) return ' ';
        return '┏';
    }

    private static char TopRight(this int[][] grid, int row, int col)
    {
        var ne = grid.At(row - 1, col + 1) == grid.At(row, col);
        var e = grid.At(row, col + 1) == grid.At(row, col);
        var n = grid.At(row - 1, col) == grid.At(row, col);
        if (e && n && !ne) return '┗';
        if (n && !e) return grid.Right(row, col);
        if (e && !n) return grid.Top(row, col);
        if (e && n && ne) return ' ';
        return '┓';
    }

    private static char BottomLeft(this int[][] grid, int row, int col)
    {
        var sw = grid.At(row + 1, col - 1) == grid.At(row, col);
        var w = grid.At(row, col - 1) == grid.At(row, col);
        var s = grid.At(row + 1, col) == grid.At(row, col);
        if (w && s && !sw) return '┓';
        if (s && !w) return grid.Left(row, col);
        if (w && !s) return grid.Bottom(row, col);
        if (w && s && sw) return ' ';
        return '┗';
    }

    private static char BottomRight(this int[][] grid, int row, int col)
    {
        var se = grid.At(row + 1, col + 1) == grid.At(row, col);
        var e = grid.At(row, col + 1) == grid.At(row, col);
        var s = grid.At(row + 1, col) == grid.At(row, col);
        if (e && s && !se) return '┏';
        if (e && !s) return grid.Bottom(row, col);
        if (s && !e) return grid.Right(row, col);
        if (s && e && se) return ' ';
        return '┛';
    }

    private static char Left(this int[][] grid, int row, int col)
    {
        return grid.At(row, col - 1) == grid.At(row, col) ? ' ' : '┃';
    }

    private static char Right(this int[][] grid, int row, int col)
    {
        return grid.At(row, col + 1) == grid.At(row, col) ? ' ' : '┃';
    }

    private static char Top(this int[][] grid, int row, int col)
    {
        return grid.At(row - 1, col) == grid.At(row, col) ? ' ' : '━';
    }

    private static char Bottom(this int[][] grid, int row, int col)
    {
        return grid.At(row + 1, col) == grid.At(row, col) ? ' ' : '━';
    }

    private static int At(this int[][] grid, int row, int col)
    {
        if (row < 0) return 0;
        if (col < 0) return 0;
        if (row >= grid.Length) return 0;
        return col >= grid[row].Length ? 0 : grid[row][col];
    }

    private static void CheckNeighbours(char c, int regionId, char[][] grid, int[][] result, int row, int col)
    {
        if (row > 0)
            if (grid[row - 1][col] == c && result[row - 1][col] == 0)
            {
                result[row - 1][col] = regionId;
                CheckNeighbours(c, regionId, grid, result, row - 1, col);
            }

        if (row < grid.Length - 1)
            if (grid[row + 1][col] == c && result[row + 1][col] == 0)
            {
                result[row + 1][col] = regionId;
                CheckNeighbours(c, regionId, grid, result, row + 1, col);
            }

        if (col > 0)
            if (grid[row][col - 1] == c && result[row][col - 1] == 0)
            {
                result[row][col - 1] = regionId;
                CheckNeighbours(c, regionId, grid, result, row, col - 1);
            }

        if (col < grid[row].Length - 1)
            if (grid[row][col + 1] == c && result[row][col + 1] == 0)
            {
                result[row][col + 1] = regionId;
                CheckNeighbours(c, regionId, grid, result, row, col + 1);
            }
    }

    private static int[][] MapPerimeters(this char[][] grid)
    {
        var result = grid.Select(row => row.Select(_ => 0).ToArray()).ToArray();
        for (var row = 0; row < grid.Length; row++)
        for (var col = 0; col < grid[row].Length; col++)
            result[row][col] = grid.Perimeter(row, col);

        return result;
    }

    private static int Perimeter(this char[][] grid, int row, int col)
    {
        var p = 0;
        var me = grid[row][col];
        if (row == 0 || grid[row - 1][col] != me) p++;
        if (col == 0 || grid[row][col - 1] != me) p++;
        if (row == grid.Length - 1 || grid[row + 1][col] != me) p++;
        if (col == grid[row].Length - 1 || grid[row][col + 1] != me) p++;
        return p;
    }
}