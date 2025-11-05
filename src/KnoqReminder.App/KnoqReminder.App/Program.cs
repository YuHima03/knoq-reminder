using KnoqReminder.App.Components;
using KnoqReminder.App.Configurations;
using KnoqReminder.Domain.Options;
using KnoqReminder.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureAppOptions(builder.Configuration);

        builder.Services.AddDbContextFactory<AppDbContext>((sp, ob) =>
        {
            var connectionString = sp.GetRequiredService<IOptions<IDbConnectionOptions>>().Value.ConnectionString;
            ob.UseMySQL(connectionString);
            if (builder.Environment.IsDevelopment())
            {
                ob.EnableDetailedErrors();
                ob.EnableSensitiveDataLogging();
            }
        });

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
