using KnoqReminder.Domain.Options;
using Microsoft.Extensions.Options;

namespace KnoqReminder.App.Configurations;

static class Configuration
{
    public static IServiceCollection ConfigureAppOptions(this IServiceCollection services, IConfiguration config)
    {
        return services
            .Configure<IDbConnectionOptions, MariaDbConnectionConfiguration>(config)
            .Configure<IKnoqApiClientOptions, KnoqApiClientConfiguration>(config)
            .Configure<ITraqApiClientOptions, TraqApiClientConfiguration>(config)
            .Configure<ITraqBotOptions, TraqBotConfiguration>(config);
    }

    /// <summary>
    /// Registers a <typeparamref name="TOptionsImplement"/> configuration as of the <typeparamref name="TOptions"/> type.
    /// </summary>
    static IServiceCollection Configure<TOptions, TOptionsImplement>(this IServiceCollection services, IConfiguration config)
        where TOptions : class
        where TOptionsImplement : class, TOptions
    {
        services.AddOptions();
        // Bind the configuration section to the implementation type.
        services.AddSingleton<IConfigureOptions<TOptionsImplement>>(new ConfigureOptions<TOptionsImplement>(config.Bind));
        // Register the implementation type as the base type.
        services
            .AddSingleton<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(config))
            .AddSingleton<IOptions<TOptions>>(sp => sp.GetRequiredService<IOptions<TOptionsImplement>>());

        return services;
    }
}
