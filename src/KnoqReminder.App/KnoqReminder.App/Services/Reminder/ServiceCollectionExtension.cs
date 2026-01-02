using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Services.Reminder;
using KnoqReminder.Utilities.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace KnoqReminder.App.Services.Reminder;

static class ServiceCollectionExtension
{
    public static IServiceCollection SetupReminderServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        // Register dependencies
        services.AddMemoryCache();
        services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.TryAddSingleton(sp =>
        {
            var provider = sp.GetRequiredService<ObjectPoolProvider>();
            return provider.CreateStringBuilderPool();
        });
        // Regiser services and options
        services.Configure<IReminderOptions, ReminderConfiguration>(configuration.GetSection(ReminderConfiguration.Position));
        services.TryAddSingleton<IReminderPublisher, ReminderPublisher>();
        services.TryAddSingleton<ReminderScheduler>();
        return services;
    }
}
