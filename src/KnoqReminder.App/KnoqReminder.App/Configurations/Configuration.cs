using KnoqReminder.Domain.Options;
using Microsoft.Extensions.Options;

namespace KnoqReminder.App.Configurations;

static class Configuration
{
    public static IServiceCollection ConfigureAppOptions(this IServiceCollection services, IConfiguration config)
    {
        return services
            .Configure<DbConnectionOptions, MariaDbConnectionConfiguration>(config);
    }

    /// <summary>
    /// Registers a <typeparamref name="TOptionsImplement"/> configuration as of the <typeparamref name="TOptions"/> type.
    /// </summary>
    static IServiceCollection Configure<TOptions, TOptionsImplement>(this IServiceCollection services, IConfiguration config)
        where TOptions : class
        where TOptionsImplement : class, TOptions
    {
        return services
            .AddOptions()
            .AddSingleton((IOptionsChangeTokenSource<TOptions>)new ConfigurationChangeTokenSource<TOptionsImplement>(config))
            .AddSingleton((IConfigureOptions<TOptions>)new ConfigureOptions<TOptionsImplement>(config.Bind));
    }
}
