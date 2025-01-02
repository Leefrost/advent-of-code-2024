namespace AdventOfCode.Days;

public static class Day4
{
    private static readonly int[] Directions = [-1, 0, 1];

    public static int Part1(string input)
    {
        var lines = File.ReadAllLines(input)
            .Select(line => line.ToArray()).ToArray();

        var total = 0;
        for (var x = 0; x < lines.Length; x++)
        for (var y = 0; y < lines[x].Length; y++)
            total += lines.SearchXmas(x, y);

        return total;
    }

    public static int Part2(string input)
    {
        var lines = File.ReadAllLines(input)
            .Select(line => line.ToArray()).ToArray();

        var total = 0;
        for (var r = 1; r < lines.Length - 1; r++)
        for (var c = 1; c < lines[r].Length - 1; c++)
            if (lines.IsCross(r, c))
                total++;

        return total;
    }

    private static int SearchXmas(this char[][] grid, int x, int y)
    {
        var hits = 0;
        foreach (var deltaRow in Directions)
        foreach (var deltaCol in Directions)
            if (grid.IsXmas(x, y, deltaRow, deltaCol))
                hits++;

        return hits;
    }

    private static bool IsXmas(this char[][] grid, int x, int y, int deltaRow, int deltaCol)
    {
        if (deltaRow == 0 && deltaCol == 0)
            return false;
        if (grid[x][y] != 'X')
            return false;
        if (x + 3 * deltaRow < 0)
            return false;
        if (x + 3 * deltaRow >= grid.Length)
            return false;
        if (y + 3 * deltaCol < 0)
            return false;
        if (y + 3 * deltaCol >= grid[0].Length)
            return false;

        return grid[x + deltaRow][y + deltaCol] == 'M'
               && grid[x + 2 * deltaRow][y + 2 * deltaCol] == 'A'
               && grid[x + 3 * deltaRow][y + 3 * deltaCol] == 'S';
    }

    private static bool IsCross(this char[][] grid, int x, int y)
    {
        if (grid[x][y] != 'A')
            return false;

        return (
            (grid[x - 1][y - 1] == 'M' && grid[x + 1][y + 1] == 'S')
            ||
            (grid[x - 1][y - 1] == 'S' && grid[x + 1][y + 1] == 'M')
        ) && (
            (grid[x - 1][y + 1] == 'M' && grid[x + 1][y - 1] == 'S')
            ||
            (grid[x - 1][y + 1] == 'S' && grid[x + 1][y - 1] == 'M')
        );
    }
}