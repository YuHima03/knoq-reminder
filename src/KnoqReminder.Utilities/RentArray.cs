using System.Buffers;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace KnoqReminder.Utilities;

struct RentArray<T>(T[] source, int length) : IDisposable
{
    T[] _source = source;
    readonly int _length = length;

    readonly bool Disposed => _source.Length == 0;

    public readonly int Length => Disposed ? 0 : _length;

    public readonly T[] SourceArray => _source;

    public readonly Span<T> Span => Disposed ? [] : _source.AsSpan(0, _length);

    public void Dispose()
    {
        var array = Interlocked.Exchange(ref _source, []);
        if (array.Length == 0)
        {
            return;
        }
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _source.AsSpan(0, _length).Clear();
        }
        ArrayPool<T>.Shared.Return(_source, false);
    }

    public RentArray<T> Resize(int length)
    {
        Guard.IsGreaterThanOrEqualTo(length, 0);
        var source = Interlocked.Exchange(ref _source, []);
        if (source.Length == 0)
        {
            return ThrowHelper.ThrowObjectDisposedException<RentArray<T>>(nameof(RentArray<T>));
        }
        Guard.IsLessThanOrEqualTo(length, source.Length);
        return new(source, length);
    }

    public readonly T[] ToArray() => Span.ToArray();
}

static class RentArray
{
    public static RentArray<T> RentAndCreate<T>(int length)
    {
        if (length == 0)
        {
            return default;
        }
        return new(ArrayPool<T>.Shared.Rent(length), length);
    }
}
