using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using KnoqReminder.Utilities;
using Microsoft.EntityFrameworkCore;
using ZLinq;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DbSetHelper
{
    const int MaxStackAllocationBytes = 1024;

    public static void ApplyChanges<TEntity, TValue>(this DbSet<TEntity> dbSet, scoped ReadOnlySpan<TValue> newState, scoped ReadOnlySpan<TEntity> previousState, Func<TEntity, TValue> valueConverter, Func<TValue, TEntity> entityFactory)
        where TEntity : class
        where TValue : unmanaged, IComparable<TValue>, IEquatable<TValue>
    {
        Guard.IsNotNull(dbSet);
        Guard.IsNotNull(valueConverter);
        Guard.IsNotNull(entityFactory);

        if ((newState.Length + previousState.Length) * Unsafe.SizeOf<IndexedValue<TValue>>() <= MaxStackAllocationBytes)
        {
            Span<IndexedValue<TValue>> buffer = stackalloc IndexedValue<TValue>[newState.Length + previousState.Length];
            execCore(dbSet, newState, previousState, buffer, valueConverter, entityFactory);
        }
        else
        {
            using var bufferArray = RentArray.RentAndCreate<IndexedValue<TValue>>(newState.Length + previousState.Length);
            var buffer = bufferArray.Span;
            execCore(dbSet, newState, previousState, buffer, valueConverter, entityFactory);
        }

        static void execCore(
            DbSet<TEntity> dbSet,
            scoped ReadOnlySpan<TValue> newState,
            scoped ReadOnlySpan<TEntity> previousState,
            scoped Span<IndexedValue<TValue>> indexedStateBuffer,
            Func<TEntity, TValue> valueConverter,
            Func<TValue, TEntity> entityFactory)
        {
            var newIndexedState = indexedStateBuffer[..newState.Length];
            var previousIndexedState = indexedStateBuffer[newState.Length..];
            newState.AsValueEnumerable()
                            .Select((v, i) => new IndexedValue<TValue>(i, v))
                            .CopyTo(newIndexedState);
            previousState.AsValueEnumerable()
                .Select((e, i) => new IndexedValue<TValue>(i, valueConverter(e)))
                .CopyTo(previousIndexedState);
            using var changes = newIndexedState.GetChanges(previousIndexedState, IndexedValueComparer<TValue>.Instance);
            foreach (var ((idx, val), chg) in changes.Span)
            {
                switch (chg)
                {
                    case EnumerableChangesHelper.Change.Added:
                        dbSet.Add(entityFactory(val));
                        break;
                    case EnumerableChangesHelper.Change.Removed:
                        dbSet.Remove(previousState[idx]);
                        break;
                }
            }
        }
    }
}

file readonly struct IndexedValue<T>(int index, T Value)
    where T : unmanaged, IComparable<T>
{
    public int Index { get; init; } = index;

    public T Value { get; init; } = Value;

    public void Deconstruct(out int index, out T value)
    {
        index = Index;
        value = Value;
    }
}

file class IndexedValueComparer<T> : Comparer<IndexedValue<T>>
    where T : unmanaged, IComparable<T>
{
    public static readonly IndexedValueComparer<T> Instance = new();

    public override int Compare(IndexedValue<T> x, IndexedValue<T> y)
    {
        return x.Value.CompareTo(y.Value);
    }
}
