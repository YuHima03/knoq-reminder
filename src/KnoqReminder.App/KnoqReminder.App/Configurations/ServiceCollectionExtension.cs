using KnoqReminder.Domain.Options;
using KnoqReminder.Utilities.Options;

namespace KnoqReminder.App.Configurations;

static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureAppOptions(this IServiceCollection services, IConfigurationRoot config)
    {
        return services
            .Configure<IDbConnectionOptions, MariaDbConnectionConfiguration>(config)
            .Configure<IDefaultLocalizationOptions, DefaultLocalizationConfiguration>(config.GetSection(DefaultLocalizationConfiguration.Position))
            .Configure<IKnoqApiClientOptions, KnoqApiClientConfiguration>(config)
            .Configure<ITraqApiClientOptions, TraqApiClientConfiguration>(config)
            .Configure<ITraqBotOptions, TraqBotConfiguration>(config);
    }
}
