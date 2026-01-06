using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;

namespace KnoqReminder.Utilities.Configuration;

sealed class DotenvConfigurationProvider(DotenvConfigurationSource source) : IniStreamConfigurationProvider(source)
{
    public override void Load()
    {
        base.Load();
        Data = ReplaceDataKeys(Data);
    }

    static Dictionary<string, string?> ReplaceDataKeys(IDictionary<string, string?> source)
    {
        StringBuilder sb = new();
        Dictionary<string, string?> replaced = [];
        foreach (var (k, v) in source)
        {
            replaced[DotenvConfigurationPathHelper.NormalizeKey(k, sb)] = v;
        }
        source.Clear();
        return replaced;
    }
}

sealed class DotenvConfigurationSource : IniStreamConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DotenvConfigurationProvider(this);
    }
}

static class DotenvConfigurationPathHelper
{
    public static string NormalizeKey(ReadOnlySpan<char> key, StringBuilder? sb = null)
    {
        sb ??= new();
        using var sections = key.Split("__");
        foreach (var sectionRange in sections)
        {
            var originalSectionName = key[sectionRange];
            using var parts = originalSectionName.Split("_");
            foreach (var partRange in parts)
            {
                var originalPart = originalSectionName[partRange];
                for (int i = 0; i < originalPart.Length; i++)
                {
                    var c = originalPart[i];
                    sb.Append((i == 0) ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c));
                }
                sb.Append(ConfigurationPath.KeyDelimiter);
            }
        }
        if (sb.Length != 0)
        {
            sb.Remove(sb.Length - 1, 1); // Remove the last delimiter
        }
        var result = sb.ToString();
        sb.Clear();
        return result;
    }
}
