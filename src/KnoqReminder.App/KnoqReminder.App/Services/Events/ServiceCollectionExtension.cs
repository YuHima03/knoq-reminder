using KnoqReminder.Domain.Services.Events;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KnoqReminder.App.Services.Events;

static class ServiceCollectionExtension
{
    public static IServiceCollection SetupEventProvider(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.TryAddSingleton<IEventProvider, EventProvider>();
        return services;
    }
}
