using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using KnoqReminder.Utilities;
using Microsoft.EntityFrameworkCore;
using ZLinq;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DbSetHelper
{
    const int MaxStackAllocationBytes = 1024;

    public static void ApplyChanges<TEntity, TValue>(
        this DbSet<TEntity> dbSet,
        scoped ReadOnlySpan<TValue> newState,
        scoped ReadOnlySpan<TEntity> previousState,
        Func<TEntity, TValue> valueConverter,
        Func<TValue, TEntity> entityFactory
    )
        where TEntity : class
        where TValue : unmanaged, IComparable<TValue>
    {
        Guard.IsNotNull(dbSet);
        Guard.IsNotNull(valueConverter);
        Guard.IsNotNull(entityFactory);
        if ((newState.Length + previousState.Length) * Unsafe.SizeOf<IndexedValue<TValue>>() <= MaxStackAllocationBytes)
        {
            Span<IndexedValue<TValue>> buffer = stackalloc IndexedValue<TValue>[newState.Length + previousState.Length];
            DbSetHelperFileInternal.ApplyChangesCore(dbSet, newState, previousState, buffer, valueConverter, entityFactory);
        }
        else
        {
            using var bufferArray = RentArray.RentAndCreate<IndexedValue<TValue>>(newState.Length + previousState.Length);
            DbSetHelperFileInternal.ApplyChangesCore(dbSet, newState, previousState, bufferArray.Span, valueConverter, entityFactory);
        }
    }

    public static void ApplyChangesSlow<TEntity, TValue>(
        this DbSet<TEntity> dbSet,
        scoped ReadOnlySpan<TValue> newState,
        scoped ReadOnlySpan<TEntity> previousState,
        Func<TEntity, TValue> valueConverter,
        Func<TValue, TEntity> entityFactory
    )
        where TEntity : class
        where TValue : struct, IComparable<TValue>
    {
        Guard.IsNotNull(dbSet);
        Guard.IsNotNull(valueConverter);
        Guard.IsNotNull(entityFactory);
        using var bufferArray = RentArray.RentAndCreate<IndexedValue<TValue>>(newState.Length + previousState.Length);
        DbSetHelperFileInternal.ApplyChangesCoreSlow(dbSet, newState, previousState, bufferArray.Span, valueConverter, entityFactory);
    }
}

file static class DbSetHelperFileInternal
{
    static void HandleChanges<TEntity, TValue>(
        DbSet<TEntity> dbSet,
        scoped ReadOnlySpan<TEntity> previousState,
        scoped ReadOnlySpan<EnumerableChangesHelper.ValueWithChange<IndexedValue<TValue>>> changes,
        Func<TValue, TEntity> entityFactory
    )
        where TEntity : class
        where TValue : struct, IComparable<TValue>
    {
        foreach (var ((idx, val), chg) in changes)
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

    static void PrepareIndexed<TEntity, TValue>(
        scoped ReadOnlySpan<TValue> newState,
        scoped ReadOnlySpan<TEntity> previousState,
        scoped Span<IndexedValue<TValue>> newIndexedState,
        scoped Span<IndexedValue<TValue>> previousIndexedState,
        Func<TEntity, TValue> valueConverter
    )
        where TValue : struct, IComparable<TValue>
    {
        newState.AsValueEnumerable()
                    .Select((v, i) => new IndexedValue<TValue>(i, v))
                    .CopyTo(newIndexedState);
        previousState.AsValueEnumerable()
            .Select((e, i) => new IndexedValue<TValue>(i, valueConverter(e)))
            .CopyTo(previousIndexedState);
    }

    public static void ApplyChangesCore<TEntity, TValue>(
            DbSet<TEntity> dbSet,
            scoped ReadOnlySpan<TValue> newState,
            scoped ReadOnlySpan<TEntity> previousState,
            scoped Span<IndexedValue<TValue>> indexedStateBuffer,
            Func<TEntity, TValue> valueConverter,
            Func<TValue, TEntity> entityFactory
    )
        where TEntity : class
        where TValue : unmanaged, IComparable<TValue>
    {
        var newIndexedState = indexedStateBuffer[..newState.Length];
        var previousIndexedState = indexedStateBuffer[newState.Length..];
        PrepareIndexed(newState, previousState, newIndexedState, previousIndexedState, valueConverter);
        using var changes = newIndexedState.GetChanges(previousIndexedState, IndexedValueComparer<TValue>.Instance);
        HandleChanges(dbSet, previousState, changes.Span, entityFactory);
    }

    public static void ApplyChangesCoreSlow<TEntity, TValue>(
            DbSet<TEntity> dbSet,
            scoped ReadOnlySpan<TValue> newState,
            scoped ReadOnlySpan<TEntity> previousState,
            scoped Span<IndexedValue<TValue>> indexedStateBuffer,
            Func<TEntity, TValue> valueConverter,
            Func<TValue, TEntity> entityFactory
    )
        where TEntity : class
        where TValue : struct, IComparable<TValue>
    {
        var newIndexedState = indexedStateBuffer[..newState.Length];
        var previousIndexedState = indexedStateBuffer[newState.Length..];
        PrepareIndexed(newState, previousState, newIndexedState, previousIndexedState, valueConverter);
        using var changes = newIndexedState.GetChangesSlow(previousIndexedState, IndexedValueComparer<TValue>.Instance);
        HandleChanges(dbSet, previousState, changes.Span, entityFactory);
    }
}

file readonly struct IndexedValue<T>(int index, T Value)
    where T : struct, IComparable<T>
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
    where T : struct, IComparable<T>
{
    public static readonly IndexedValueComparer<T> Instance = new();

    public override int Compare(IndexedValue<T> x, IndexedValue<T> y)
    {
        return x.Value.CompareTo(y.Value);
    }
}
