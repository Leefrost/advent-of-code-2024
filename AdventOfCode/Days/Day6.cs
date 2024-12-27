namespace AdventOfCode.Days;

public static class Directions
{
}

public static class Day6
{
    private const char E = 'E';
    private const char W = 'W';
    private const char N = 'N';
    private const char S = 'S';
    
    public static int Part1(string input)
    {
        var grid = File.ReadAllLines(input)
            .Select(line => line.ToArray()).ToArray();

        var visited = new HashSet<(int x, int y)>();
        var start = grid.StartAt();
        visited.Add(start);
        
        var direction = N;
        var current = start;
        while (true)
        {
            var next = current.MoveTo(direction);
            if (!grid.IsOutOfBounce(next))
                break;
            if (grid[next.row][next.col] == '#')
                direction = direction.Rotate();
            else
            {
                current = next;
                visited.Add(current);
            }
        }

        return visited.Count;
    }
    
    public static int Part2(string input)
    {
        var grid = File.ReadAllLines(input)
            .Select(line => line.ToArray()).ToArray();

        var visited = new HashSet<(int x, int y)>();
        var start = grid.StartAt();
        visited.Add(start);
        
        var direction = N;
        var current = start;
        while (true)
        {
            var next = current.MoveTo(direction);
            if (!grid.IsOutOfBounce(next))
                break;
            if (grid[next.row][next.col] == '#')
                direction = direction.Rotate();
            else
            {
                current = next;
                visited.Add(current);
            }
        }

        var blocked = new HashSet<(int x, int y)>();
        foreach (var visit in visited)
        {
            if(visit == start) 
                continue;
            
            if (grid.SubGrid(visit).HasLoop(start))
                blocked.Add(visit);
        }
        
        return blocked.Count;
    }
    
    private static char Rotate(this char direction)
    {
        return direction switch
        {
            N => E,
            E => S,
            S => W,
            W => N,
            _ => '?'
        };
    }

    private static (int row, int col) MoveTo(this (int row, int col) point, char direction)
    {
        return direction switch
        {
            S => (point.row + 1, point.col),
            E => (point.row, point.col + 1),
            W => (point.row, point.col - 1),
            _ => (point.row - 1, point.col)
        };
    }

    private static bool IsOutOfBounce(this char[][] grid, (int row, int col) point)
    {
        return point.row >= 0 && point.row < grid.Length 
                              && point.col >= 0 && point.col < grid[0].Length;
    }

    private static char[][] SubGrid(this char[][] grid, (int row, int col) point)
    {
        var clone = grid.Select(line => line.ToArray()).ToArray();
        clone[point.row][point.col] = '#';
        return clone;
    }

    private static bool HasLoop(this char[][] grid, (int row, int col) point)
    {
        var direction = N;
        while (true)
        {
            var next = point.MoveTo(direction);
            if(!grid.IsOutOfBounce(next))
                return false;
            if (grid[next.row][next.col] == direction)
                return true;
            if (grid[next.row][next.col] == '#')
                direction = direction.Rotate();
            else
            {
                point = next;
                grid[point.row][point.col] = direction;
            }
        }
    }
    
    private static (int row, int col) StartAt(this char[][] grid)
    {
        for (var row = 0; row < grid.Length; row++)
        for (var col = 0; col < grid[0].Length; col++)
            if (grid[row][col] == '^')
                return (row, col);

        return (-1, -1);
    }
}