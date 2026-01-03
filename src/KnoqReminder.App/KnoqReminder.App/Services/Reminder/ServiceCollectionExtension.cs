using KnoqReminder.App.Configurations;
using KnoqReminder.App.Helpers;
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
        services.AddDefaultObjectPool();
        services.TryAddSingleton(sp =>
        {
            var provider = sp.GetRequiredService<ObjectPoolProvider>();
            return provider.CreateStringBuilderPool();
        });
        // Register services and options
        services.AddOptions<IReminderOptions, ReminderConfiguration>()
            .Bind(configuration.GetSection(ReminderConfiguration.Position))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.TryAddSingleton<IReminderPublisher, ReminderPublisher>();
        services.AddHostedService<ReminderScheduler>();
        return services;
    }
}
