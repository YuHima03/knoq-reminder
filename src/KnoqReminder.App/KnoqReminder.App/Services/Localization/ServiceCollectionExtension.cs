using KnoqReminder.Domain.Services.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KnoqReminder.App.Services.Localization;

static class ServiceCollectionExtension
{
    public static IServiceCollection SetupDefaultLocalization(this IServiceCollection services)
    {
        services.TryAddSingleton<ILocalTimeProvider, LocalTimeProvider>();
        return services;
    }
}
