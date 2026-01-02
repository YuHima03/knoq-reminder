using KnoqReminder.App.Configurations;
using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Services.DiscordWebhook;
using KnoqReminder.Utilities.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KnoqReminder.App.Services.DiscordWebhook;

static class ServiceCollectionExtension
{
    public static IServiceCollection SetupDiscordWebhookPublisher(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddHttpClient();
        services.Configure<IDiscordWebhookOptions, DiscordWebhookConfiguration>(config.GetSection(DiscordWebhookConfiguration.Position));
        services.TryAddSingleton<IDiscordWebhookPublisher, DiscordWebhookPublisher>();
        return services;
    }
}
