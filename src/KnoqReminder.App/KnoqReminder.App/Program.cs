using KnoqReminder.App.Components;
using KnoqReminder.App.Configurations;
using KnoqReminder.App.Services.Localization;
using KnoqReminder.App.Services.Reminder;
using KnoqReminder.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .ConfigureAppOptions(builder.Configuration)
            .SetupDefaultLocalization(builder.Configuration)
            .SetupReminderServices(builder.Configuration)
            .SetupRepository(builder.Configuration, builder.Environment);

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
