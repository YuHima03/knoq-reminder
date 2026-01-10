using KnoqReminder.App.Components;
using KnoqReminder.App.Configurations;
using KnoqReminder.App.Services;
using KnoqReminder.App.Services.DiscordWebhook;
using KnoqReminder.App.Services.Events;
using KnoqReminder.App.Services.Localization;
using KnoqReminder.App.Services.Reminder;
using KnoqReminder.Infrastructure.Database;
using KnoqReminder.Utilities.Configuration;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddKeyNormalizedEnvironmentVariables()
            .AddEnvFiles(true, ".env", $"{builder.Environment.EnvironmentName}.env")
            .AddEnvFiles(false, builder.Configuration["env-files"]?.Split(';'));

        builder.Services
            .ConfigureAppOptions(builder.Configuration)
            .SetupDefaultLocalization(builder.Configuration)
            .SetupDiscordWebhookPublisher(builder.Configuration)
            .SetupEventProvider()
            .SetupKnoqClient(builder.Configuration)
            .SetupReminderServices(builder.Configuration)
            .SetupRepository(builder.Configuration, builder.Environment)
            .SetupTraqClient(builder.Configuration);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(KnoqReminder.App.Client._Imports).Assembly);

        app.Run();
    }
}
