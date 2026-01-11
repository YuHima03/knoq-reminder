using System.ComponentModel.DataAnnotations;
using KnoqReminder.Domain.Options;
using KnoqReminder.Infrastructure.Database;
using KnoqReminder.Utilities.Options;
using Microsoft.Extensions.Options;

namespace KnoqReminder.App.Configurations;

static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureAppOptions(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddOptions<ITraqBotOptions, TraqBotConfiguration>()
            .Bind(config.GetSection(TraqBotConfiguration.Position))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }

    public static IServiceCollection SetupRepository(this IServiceCollection services, IConfigurationRoot config, IHostEnvironment? environment = null)
    {
        services.AddRepository(environment);
        services.ConfigureRepository(config, validation: false);
        services.AddOptions<NsMariaDbConnectionConfiguration>()
            .Bind(config);
        services.AddSingleton<IOptions<IDbConnectionOptions>>(sp =>
        {
            var nsConfig = sp.GetRequiredService<IOptions<NsMariaDbConnectionConfiguration>>();
            if (nsConfig.Value.IsValid)
            {
                return nsConfig;
            }
            var defaultConfig = sp.GetRequiredService<IOptions<MariaDbConnectionConfiguration>>();
            var defaultConfigValue = defaultConfig.Value;
            Validator.ValidateObject(defaultConfigValue, new ValidationContext(defaultConfigValue), validateAllProperties: true);
            return defaultConfig;
        });
        return services;
    }
}
