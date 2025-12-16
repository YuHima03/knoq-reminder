
using KnoqReminder.Domain.Options;
using Microsoft.Extensions.Options;

namespace KnoqReminder.App.Services.Reminder;

public class ReminderScheduler(
    IOptions<IReminderOptions> options
    )
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(options.Value.SchedulingInterval);
        do
        {

        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }
}
