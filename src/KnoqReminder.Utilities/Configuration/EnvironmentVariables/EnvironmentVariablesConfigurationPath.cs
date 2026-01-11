using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using ZLinq;

namespace KnoqReminder.Utilities.Configuration.EnvironmentVariables;

static class EnvironmentVariablesConfigurationPath
{
    public const string KeyDelimiter = "__";

    public const string WordDelimiter = "_";

    public static string NormalizeKey(scoped ReadOnlySpan<char> key)
    {
        if (key is [])
        {
            return string.Empty;
        }
        Span<char> buffer = stackalloc char[key.Length];
        var charsWritten = NormalizeKey(key, buffer);
        return new string(buffer[..charsWritten]);
    }

    public static int NormalizeKey(scoped ReadOnlySpan<char> source, scoped Span<char> destination)
    {
        Guard.IsGreaterThanOrEqualTo(destination.Length, source.Length);
        using var sections = source.Split(KeyDelimiter);
        int charsWritten = 0;
        foreach (var partRange in sections)
        {
            charsWritten += NormalizeSectionName(source[partRange], destination[charsWritten..]);
            if (charsWritten == destination.Length)
            {
                // The key string has no more sections.
                return charsWritten;
            }
            destination[charsWritten] = ConfigurationPath.KeyDelimiter[0];
            charsWritten++;
        }
        // Remove the last delimiter.
        return charsWritten - 1;
    }

    public static int NormalizeSectionName(scoped ReadOnlySpan<char> source, scoped Span<char> destination)
    {
        Guard.IsGreaterThanOrEqualTo(destination.Length, source.Length);
        using var words = source.Split(WordDelimiter);
        int charsWritten = 0;
        foreach (var partRange in words)
        {
            charsWritten += source[partRange]
                .Trim()
                .AsValueEnumerable()
                .Select((c, i) => (i == 0) ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c))
                .CopyTo(destination[charsWritten..]);
        }
        return charsWritten;
    }
}
