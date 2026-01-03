using Knoq.Extensions.Authentication;
using KnoqReminder.App.Configurations;
using KnoqReminder.App.Services.Urls;
using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Services.Urls;
using KnoqReminder.Utilities.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Traq;

namespace KnoqReminder.App.Services;

static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupKnoqClient(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddOptions<IKnoqClientOptions, KnoqClientConfiguration>()
            .Bind(config)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddAuthenticatedKnoqApiClient(
            configureKnoq: (sp, o) =>
            {
                var options = sp.GetRequiredService<IOptions<IKnoqClientOptions>>().Value;
                o.BaseAddress = options.ApiBaseUrl.OriginalString;
            },
            configureTraqAuth: (sp, auth) =>
            {
                var options = sp.GetRequiredService<IOptions<IKnoqClientOptions>>().Value;
                auth.UsePasswordAuthentication(options.Username, options.Password);
            });
        services.TryAddSingleton<IKnoqUrlProvider, KnoqUrlProvider>();
        return services;
    }

    public static IServiceCollection SetupTraqClient(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddOptions<ITraqClientOptions, TraqClientConfiguration>()
            .Bind(config)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddTraqApiClient((sp, o) =>
        {
            var options = sp.GetRequiredService<IOptions<ITraqClientOptions>>().Value;
            o.BaseAddress = options.ApiBaseUrl.OriginalString;
            o.BearerAuthToken = options.AccessToken;
        });
        return services;
    }
}
