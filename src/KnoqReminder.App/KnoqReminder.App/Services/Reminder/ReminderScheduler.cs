
using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Localization;
using Microsoft.Extensions.Options;

namespace KnoqReminder.App.Services.Reminder;

public class ReminderScheduler(
    IEventProvider eventProvider,
    ILocalTimeProvider localTimeProvider,
    IOptions<IReminderOptions> options
    )
    : BackgroundService
{
    DateTimeOffset _lastCollectedAt = DateTimeOffset.MinValue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = options.Value.SchedulingInterval;
        var aotTimeSpan = options.Value.AotReminderTimeBeforeEvent;
        using PeriodicTimer timer = new(interval);
        do
        {
            var utcNow = DateTimeOffset.UtcNow;
            var upcomingEvents = await eventProvider.GetEventsAsync(
                utcNow - aotTimeSpan,
                utcNow + interval - aotTimeSpan,
                stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }
}
