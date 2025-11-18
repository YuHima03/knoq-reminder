using Microsoft.Extensions.Caching.Memory;

namespace KnoqReminder.Utilities;

readonly struct KeyedMemoryCacheEntryOptions
{
    public required string Key { get; init; }

    public required MemoryCacheEntryOptions Options { get; init; }

    public string GetMemoryCacheKey<T>(T index) where T : ISpanFormattable
    {
        return $"{Key}[{index}]";
    }
}
