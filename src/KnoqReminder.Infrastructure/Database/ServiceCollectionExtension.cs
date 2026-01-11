using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Repositories;
using KnoqReminder.Infrastructure.Database.Implements;
using KnoqReminder.Utilities.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KnoqReminder.Infrastructure.Database;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRepository(this IServiceCollection services, IHostEnvironment? environment = null)
    {
        services.AddDbContextFactory<AppDbContext>((sp, ob) =>
        {
            var connectionString = sp.GetRequiredService<IOptions<IDbConnectionOptions>>().Value.ConnectionString;
            ob.UseMySQL(connectionString);
            if (environment?.IsDevelopment() is true)
            {
                ob.EnableDetailedErrors();
                ob.EnableSensitiveDataLogging();
            }
        });
        services.TryAddSingleton<IRepositoryProvider, RepositoryProviderImplement>();
        return services;
    }

    public static IServiceCollection ConfigureRepository(this IServiceCollection services, IConfigurationRoot config, bool validation = true)
    {
        var ob = services.AddOptions<IDbConnectionOptions, MariaDbConnectionConfiguration>()
            .Bind(config.GetSection(MariaDbConnectionConfiguration.Position));
        if (validation)
        {
            ob.ValidateDataAnnotations()
                .ValidateOnStart();
        }
        return services;
    }
}
