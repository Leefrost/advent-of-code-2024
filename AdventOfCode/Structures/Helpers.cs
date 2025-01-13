namespace AdventOfCode.Structures;

public static class Helpers
{
    public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);

        return source;
    }
}