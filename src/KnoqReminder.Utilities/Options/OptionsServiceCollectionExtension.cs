using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace KnoqReminder.Utilities.Options;

public static class OptionsServiceCollectionExtension
{
    public static OptionsBuilder<TOptionsImplement> AddOptions<TOptions, TOptionsImplement>(this IServiceCollection services)
        where TOptions : class
        where TOptionsImplement : class, TOptions
    {
        services.TryAddSingleton<IOptions<TOptions>>(sp => sp.GetRequiredService<IOptions<TOptionsImplement>>());
        return services.AddOptions<TOptionsImplement>();
    }

    /// <summary>
    /// Registers a <typeparamref name="TOptionsImplement"/> configuration as of the <typeparamref name="TOptions"/> type.
    /// </summary>
    public static IServiceCollection Configure<TOptions, TOptionsImplement>(this IServiceCollection services, IConfiguration config)
        where TOptions : class
        where TOptionsImplement : class, TOptions
    {
        services.AddOptions();
        // Bind the configuration section to the implementation type.
        services.TryAddSingleton<IConfigureOptions<TOptionsImplement>>(new ConfigureOptions<TOptionsImplement>(config.Bind));
        // Register the implementation type as the base type.
        services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(config));
        services.TryAddSingleton<IOptions<TOptions>>(sp => sp.GetRequiredService<IOptions<TOptionsImplement>>());
        return services;
    }
}
