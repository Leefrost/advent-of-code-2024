namespace AdventOfCode.Extensions;

public static class Helpers
{
    public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);

        return source;
    }
    
    public static int IndexOfMin<T, TV>(this T[] source, Func<T, TV> selector)
        where TV : IComparable<TV>
    {
        var min = 0;
        for (var i = 1; i < source.Length; i++)
            if (selector(source[i]).CompareTo(selector(source[min])) < 0)
                min = i;

        return min;
    }
}