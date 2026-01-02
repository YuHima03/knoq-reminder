using KnoqReminder.Domain.Options;
using KnoqReminder.Utilities.Options;

namespace KnoqReminder.App.Configurations;

static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureAppOptions(this IServiceCollection services, IConfigurationRoot config)
    {
        return services
            .Configure<IKnoqClientOptions, KnoqClientConfiguration>(config)
            .Configure<ITraqApiClientOptions, TraqApiClientConfiguration>(config)
            .Configure<ITraqBotOptions, TraqBotConfiguration>(config);
    }
}
