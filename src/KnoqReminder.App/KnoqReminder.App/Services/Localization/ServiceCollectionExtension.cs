using KnoqReminder.App.Configurations;
using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Services.Localization;
using KnoqReminder.Utilities.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KnoqReminder.App.Services.Localization;

static class ServiceCollectionExtension
{
    public static IServiceCollection SetupDefaultLocalization(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddOptions<IDefaultLocalizationOptions, DefaultLocalizationConfiguration>()
            .Bind(config.GetSection(DefaultLocalizationConfiguration.Position))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.TryAddSingleton<ILocalTimeProvider, LocalTimeProvider>();
        return services;
    }
}
