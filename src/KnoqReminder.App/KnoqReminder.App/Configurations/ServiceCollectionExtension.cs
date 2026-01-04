using KnoqReminder.Domain.Options;
using KnoqReminder.Utilities.Options;

namespace KnoqReminder.App.Configurations;

static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureAppOptions(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddOptions<ITraqBotOptions, TraqBotConfiguration>()
            .Bind(config)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}
