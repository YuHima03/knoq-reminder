using KnoqReminder.Domain.Repositories;
using KnoqReminder.Domain.Repositories.Models;

namespace KnoqReminder.App.Services.Reminder;

public class ReminderScheduler(
    IUserReminderRepository userReminderRepository
    )
    : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lastRunAt = DateTimeOffset.MinValue;
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));
        do // Run every minute
        {
            var timeToRunAfter = lastRunAt.AddMinutes(1);
            var utcNow = DateTimeOffset.UtcNow;
            while (utcNow < timeToRunAfter)
            {
                utcNow = DateTimeOffset.UtcNow;
            }
            // Ensured: {lastRunAt:HH:mm} < {utcNow:HH:mm}
            TimeOnly utcNowTimeOnly = new(utcNow.Ticks % TimeSpan.TicksPerDay);
            DailyReminderTime utcNowReminderTime = new(utcNowTimeOnly);
            var dailyRemindersLookup = (await userReminderRepository.GetUserDailyRemindersAsync(utcNowReminderTime, utcNowReminderTime, stoppingToken).ConfigureAwait(false)).ToLookup(r => r.ReminderId);
            var aotReminders = await userReminderRepository.GetUserAotRemindersAsync(AheadOfTimeReminderTime.MinValue, AheadOfTimeReminderTime.MaxValue, stoppingToken).ConfigureAwait(false);

        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }

    async ValueTask<ILookup<Guid, UserDailyReminder>> RetrieveUserDailyRemindersAsync(DailyReminderTime time, CancellationToken cancellationToken = default)
    {
        return (await userReminderRepository.GetUserDailyRemindersAsync(time, time, cancellationToken)).ToLookup(r => r.ReminderId);
    }
}
