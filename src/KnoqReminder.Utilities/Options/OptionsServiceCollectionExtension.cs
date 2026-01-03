using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace KnoqReminder.Utilities.Options;

public static class OptionsServiceCollectionExtension
{
    public static OptionsBuilder<TOptionsImplement> AddOptions<TOptions, TOptionsImplement>(this IServiceCollection services)
        where TOptions : class
        where TOptionsImplement : class, TOptions
    {
        services.TryAddSingleton<IOptions<TOptions>>(sp => sp.GetRequiredService<IOptions<TOptionsImplement>>());
        return services.AddOptions<TOptionsImplement>();
    }
}
