using System.Collections;

namespace AdventOfCode.Structures;

public class Map2<T> : IEnumerable<T>
{
    public const int UP = 0;
    public const int RIGHT = 1;
    public const int DOWN = 2;
    public const int LEFT = 3;

    public Map2(T[] data, int columns)
    {
        Columns = columns;
        Rows = data.Length / columns;
        Data = data;
        Directions = [-Columns, 1, Columns, -1];
    }

    public T this[int loc]
    {
        get => Data[loc];
        set => Data[loc] = value;
    }

    public T this[int x, int y]
    {
        get => Data[D2toD1(x, y)];
        set => Data[D2toD1(x, y)] = value;
    }

    public int Rows { get; }
    public int Columns { get; }
    public T[] Data { get; }

    public int[] Directions { get; }

    public IEnumerator<T> GetEnumerator()
    {
        return Data.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Data.GetEnumerator();
    }

    public int InvDdx(int ddx)
    {
        return (ddx + 2) % Directions.Length;
    }

    public int Next(int location, int ddx)
    {
        return location + Directions[ddx];
    }

    public void FillBorders(T value)
    {
        for (var x = 0; x < Columns; x++)
            this[x, 0] = this[x, Rows - 1] = value;

        for (var y = 0; y < Rows; y++)
            this[0, y] = this[Columns - 1, y] = value;
    }

    public TBuffer[] CreateBuffer<TBuffer>(TBuffer defaultValue = default!)
    {
        var buffer = new TBuffer[Data.Length];
        Array.Fill(buffer, defaultValue);
        return buffer;
    }

    public Map2<T> Copy()
    {
        var buffer = new T[Data.Length];
        Array.Copy(Data, buffer, Data.Length);

        return new Map2<T>(buffer, Columns);
    }

    public void Draw(Func<T, int, string>? formatter = null)
    {
        var f = formatter ?? DefaultFormat;

        for (var i = 0; i < Data.Length; i += Columns)
        {
            for (var k = i; k < i + Columns; k++)
                Console.Write(f(Data[k], k));

            Console.WriteLine();
        }
    }

    public (int location, int direction) LD2L(int ld)
    {
        return (ld / Directions.Length, ld % Directions.Length);
    }

    public int L2LD(int location, int direction)
    {
        return Directions.Length * location + direction;
    }

    public int D2toD1(int x, int y)
    {
        return y * Columns + x;
    }

    public (int x, int y) D1toD2(int loc)
    {
        return (loc % Columns, loc / Columns);
    }

    public int Manhattan(int from, int to)
    {
        var (cx, cy) = D1toD2(from);
        var (ex, ey) = D1toD2(to);

        return Math.Abs(cx - ex) + Math.Abs(cy - ey);
    }

    public static Map2<T> Empty(int columns, int rows)
    {
        return new Map2<T>(new T[rows * columns], columns);
    }

    public static Map2<T> WithBorders(T[] data, int columns, T borderValue)
    {
        var rows = data.Length / columns;

        var map = Empty(columns + 2, rows + 2);
        map.FillBorders(borderValue);

        for (var y = 0; y < rows; y++)
        for (var x = 0; x < columns; x++)
            map[x + 1, y + 1] = data[y * columns + x];

        return map;
    }

    #region Private methods

    private string DefaultFormat(T value, int location)
    {
        return value?.ToString() ?? string.Empty;
    }

    #endregion
}