using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace KnoqReminder.Utilities.Collections;

public static class MemoryExtensions
{
    public static bool TryGetReadOnlySpan(this IEnumerable<char> source, out ReadOnlySpan<char> span)
    {
        if (source is string str)
        {
            span = str.AsSpan();
            return true;
        }
        return TryGetReadOnlySpan<char>(source, out span);
    }

    public static bool TryGetReadOnlySpan<TKey>(this IEnumerable<TKey> source, out ReadOnlySpan<TKey> span)
    {
        if (source.TryGetSpan(out var s))
        {
            span = s;
            return true;
        }
        else if (source is ImmutableArray<TKey> immArray)
        {
            span = immArray.AsSpan();
            return true;
        }
        span = default;
        return false;
    }

    public static bool TryGetSpan<TKey>(this IEnumerable<TKey> source, out Span<TKey> span)
    {
        if (source is TKey[] array)
        {
            span = array.AsSpan();
            return true;
        }
        else if (source is List<TKey> list)
        {
            span = CollectionsMarshal.AsSpan(list);
            return true;
        }
        else if (source is ArraySegment<TKey> arraySegment)
        {
            span = arraySegment.AsSpan();
            return true;
        }
        span = default;
        return false;
    }
}
