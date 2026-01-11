using Microsoft.Extensions.Configuration;

namespace KnoqReminder.Utilities.Configuration.EnvironmentVariables;

sealed class KeyNormalizedEnvironmentVariablesConfigurationSource(Predicate<string>? keyFilter = null) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new KeyNormalizedEnvironmentVariablesConfigurationProvider(keyFilter);
    }
}
