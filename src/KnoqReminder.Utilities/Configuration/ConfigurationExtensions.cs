using KnoqReminder.Utilities.Helpers;
using Microsoft.Extensions.Configuration;

namespace KnoqReminder.Utilities.Configuration;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddEnvFiles(this IConfigurationBuilder builder, bool areOptional, params ReadOnlySpan<string> paths)
    {
        foreach (var p in paths)
        {
            var fullPath = Path.GetFullPath(p);
            if (!File.Exists(fullPath))
            {
                if (areOptional)
                {
                    continue;
                }
                GenericThrowHelper.Throw<FileNotFoundException>($"Configuration file not found: {fullPath}");
            }
            using var stream = File.OpenRead(fullPath);
            builder.Add(new DotenvConfigurationSource() { Stream = stream });
        }
        return builder;
    }
}
