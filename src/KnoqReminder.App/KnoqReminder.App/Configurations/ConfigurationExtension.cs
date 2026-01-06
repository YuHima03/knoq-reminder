using KnoqReminder.Utilities.Helpers;

namespace KnoqReminder.App.Configurations;

static class ConfigurationExtension
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
            builder.AddIniStream(stream);
        }
        return builder;
    }
}
