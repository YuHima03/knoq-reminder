using KnoqReminder.Domain.Services.Reminder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace KnoqReminder.App.Services.Reminder;

static class ServiceCollectionExtension
{
    public static IServiceCollection SetupReminderServices(this IServiceCollection services)
    {
        services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.TryAddSingleton(sp =>
        {
            var provider = sp.GetRequiredService<ObjectPoolProvider>();
            return provider.CreateStringBuilderPool();
        });
        services.TryAddSingleton<IReminderPublisher, ReminderPublisher>();
        return services;
    }
}
