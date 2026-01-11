using System.Runtime.InteropServices;

namespace KnoqReminder.Utilities.Collections;

public static class ListExtensions
{
    public static void RemoveAllUnstable<T>(this List<T> list, Predicate<T> match)
    {
        var span = CollectionsMarshal.AsSpan(list);
        var keeping = span;
        for (int i = 0; i < keeping.Length; i++)
        {
            var item = keeping[i];
            if (match(item))
            {
                keeping[i] = keeping[^1];
                keeping = keeping[..^1];
            }
        }
        var removeCount = span.Length - keeping.Length;
        if (removeCount != 0)
        {
            list.RemoveRange(keeping.Length, removeCount);
        }
    }
}
