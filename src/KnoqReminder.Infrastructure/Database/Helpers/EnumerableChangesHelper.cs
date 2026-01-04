using System.Runtime.CompilerServices;
using KnoqReminder.Utilities;
using ZLinq;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class EnumerableChangesHelper
{
    public enum Change
    {
        None = 0,
        Added = 1,
        Removed = -1
    }

    public readonly struct ValueWithChange<T>(T value, Change change)
    {
        public T Value { get; init; } = value;

        public Change Change { get; init; } = change;

        public void Deconstruct(out T value, out Change change)
        {
            value = Value;
            change = Change;
        }
    }

    const int MaxStackAllocationBytes = 1024;

    public static RentArray<ValueWithChange<T>> GetChanges<T>(this scoped ReadOnlySpan<T> current, scoped ReadOnlySpan<T> previous, IComparer<T>? comparer = default) where T : unmanaged
    {
        if (previous.Length * 2 * Unsafe.SizeOf<T>() > MaxStackAllocationBytes)
        {
            return GetChangesSlow(current, previous, comparer);
        }
        Span<T> bufPrev = stackalloc T[previous.Length];
        Span<T?> bufPrevRemains = stackalloc T?[previous.Length];
        previous.CopyTo(bufPrev);
        return GetChangesCore(current, bufPrev, bufPrevRemains, comparer ?? Comparer<T>.Default);
    }

    public static RentArray<ValueWithChange<T>> GetChangesSlow<T>(this scoped ReadOnlySpan<T> current, scoped ReadOnlySpan<T> previous, IComparer<T>? comparer = default) where T : struct
    {
        using var bufPrevArray = RentArray.RentAndCreate<T>(previous.Length);
        using var bufPrevRemainsArray = RentArray.RentAndCreate<T?>(previous.Length);
        Span<T> bufPrev = bufPrevArray.Span;
        Span<T?> bufPrevRemains = bufPrevRemainsArray.Span;
        previous.CopyTo(bufPrev);
        return GetChangesCore(current, bufPrev, bufPrevRemains, comparer ?? Comparer<T>.Default);
    }

    static RentArray<ValueWithChange<T>> GetChangesCore<T>(scoped ReadOnlySpan<T> current, scoped Span<T> prev, scoped Span<T?> prevRemains, IComparer<T> comparer) where T : struct
    {
        prev.Sort(comparer);
        prev.AsValueEnumerable().Select(x => (T?)x).CopyTo(prevRemains);

        var result = RentArray.RentAndCreate<ValueWithChange<T>>(current.Length + prev.Length);
        var resSpan = result.Span;
        for (int i = 0; i < current.Length; i++)
        {
            var x = current[i];
            var otherIdx = prev.BinarySearch(x, comparer);
            if (otherIdx >= 0)
            {
                resSpan[i] = new(x, Change.None);
                prevRemains[otherIdx] = null;
            }
            else
            {
                resSpan[i] = new(x, Change.Added);
            }
        }
        int remCnt = 0;
        var remSpan = resSpan[current.Length..];
        foreach (var x in prevRemains.AsValueEnumerable().Where(y => y is not null))
        {
            remSpan[remCnt++] = new(x!.Value, Change.Removed);
        }
        return result.Resize(current.Length + remCnt);
    }
}
