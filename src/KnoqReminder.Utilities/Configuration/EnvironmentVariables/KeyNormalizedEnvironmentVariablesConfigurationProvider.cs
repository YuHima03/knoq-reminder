using System.Collections.Frozen;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace KnoqReminder.Utilities.Configuration.EnvironmentVariables;

sealed class KeyNormalizedEnvironmentVariablesConfigurationProvider(Predicate<string>? keyFilter = null) : EnvironmentVariablesConfigurationProvider
{
    public override void Load()
    {
        base.Load();
        Data = NormalizeKeys(Data);
    }

    FrozenDictionary<string, string?> NormalizeKeys(IDictionary<string, string?> source)
    {
        var filtered = keyFilter is null
            ? source
            : source.Where(kvp => keyFilter(kvp.Key));
        var keysReplaced = filtered.ToFrozenDictionary(
            kvp => NormalizeMixedKey(kvp.Key),
            kvp => kvp.Value);
        source.Clear();
        return keysReplaced;
    }

    static string NormalizeMixedKey(scoped ReadOnlySpan<char> source)
    {
        Span<char> buffer = stackalloc char[source.Length];
        using var sectionsSplitByDefaultDelimiter = source.Split(ConfigurationPath.KeyDelimiter);
        int charsWritten = 0;
        foreach (var partRange in sectionsSplitByDefaultDelimiter)
        {
            charsWritten += EnvironmentVariablesConfigurationPath.NormalizeSectionName(source[partRange], buffer[charsWritten..]);
            if (charsWritten == source.Length)
            {
                // The key string has no more sections.
                return new string(buffer[..charsWritten]);
            }
            buffer[charsWritten] = ConfigurationPath.KeyDelimiter[0];
            charsWritten++;
        }
        // Remove the last delimiter.
        return new string(buffer[..(charsWritten - 1)]);
    }
}
