using System.Diagnostics.CodeAnalysis;
using ZLinq;

namespace KnoqReminder.Utilities.Collections;

public static class DictionaryExtensions
{
    public static bool TryFindValue<TValue>(this IDictionary<string, TValue> dict, string key, StringComparison comparison, [MaybeNullWhen(false)] out TValue value)
    {
        if (dict.TryGetValue(key, out value))
        {
            return true;
        }
        Span<TValue> buffer = new(ref value!);
        var count = dict.AsValueEnumerable()
            .Where(kv => kv.Key.Equals(key, comparison))
            .Select(kv => kv.Value)
            .Take(1)
            .CopyTo(buffer);
        return count == 1;
    }
}
