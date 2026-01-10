using System.Collections.Frozen;
using KnoqReminder.Utilities.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Ini;

namespace KnoqReminder.Utilities.Configuration.Dotenv;

sealed class DotenvConfigurationProvider(DotenvConfigurationSource source) : IniStreamConfigurationProvider(source)
{
    public override void Load()
    {
        base.Load();
        Data = NormalizeKeys(Data);
    }

    static FrozenDictionary<string, string?> NormalizeKeys(IDictionary<string, string?> source)
    {
        var keysReplaced = source.ToFrozenDictionary(
            kvp => EnvironmentVariablesConfigurationPath.NormalizeKey(kvp.Key),
            kvp => kvp.Value);
        source.Clear();
        return keysReplaced;
    }
}
