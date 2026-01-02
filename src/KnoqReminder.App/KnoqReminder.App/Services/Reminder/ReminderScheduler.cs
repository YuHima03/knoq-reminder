using System.Collections.Immutable;
using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Reminder.Models;
using KnoqReminder.Domain.Repositories;
using KnoqReminder.Domain.Repositories.Models;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Reminder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Traq;
using ZLinq;

namespace KnoqReminder.App.Services.Reminder;

sealed partial class ReminderScheduler(
    IOptions<IReminderOptions> options,
    IMemoryCache cache,
    IEventProvider eventProvider,
    ILogger<ReminderScheduler> logger,
    IReminderPublisher reminderPublisher,
    TraqApiClient traq,
    IUserReminderRepository userReminderRepository
    )
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lastRunAt = DateTimeOffset.MinValue;
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(20));
        do
        {
            var utcNow = DateTimeOffset.UtcNow;
            if (lastRunAt.AddMinutes(1) <= utcNow) // Runs every minute
            {
                CancellationTokenSource cts = new(options.Value.RemindingTaskTimeout);
                var ct = cts.Token;
                ct.Register(() => LoggerExtensions.LogWarning_ReminderTimeOut(logger, options.Value.RemindingTaskTimeout));
                _ = ExecuteCoreAsync(utcNow, ct);
                lastRunAt = utcNow;
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }

    async Task ExecuteCoreAsync(DateTimeOffset time, CancellationToken cancellationToken = default)
    {
        var utcTime = time.ToUniversalTime();
        var utcTimeMinute = utcTime - TimeSpan.FromTicks(utcTime.Ticks % TimeSpan.TicksPerMinute);
        var eventsToday = await eventProvider.GetEventsAsync(utcTimeMinute, utcTimeMinute.AddDays(1), cancellationToken).ConfigureAwait(false);

        var dailyReminders = (await userReminderRepository.GetUserDailyRemindersAsync(TimeOnly.FromDateTime(utcTimeMinute.UtcDateTime), cancellationToken).ConfigureAwait(false))
            .ToAsyncEnumerable()
            .Select(async (dailyReminder, ct) =>
            {
                var reminderOverview = await userReminderRepository.GetUserReminderOverviewAsync(dailyReminder.ReminderId, ct).ConfigureAwait(false);
                var eventsToRemind = FilterEvents(
                    eventsToday,
                    dailyReminder.UserId,
                    reminderOverview.RemindsOpenEvents is ReminderOptionsForOpenEvents.RemindsDaily or ReminderOptionsForOpenEvents.RemindsDailyAndAheadOfTime,
                    reminderOverview.RemindsWhenAbsent is ReminderOptionsWhenUserAbsent.RemindsDaily or ReminderOptionsWhenUserAbsent.RemindsDailyAndAheadOfTime,
                    reminderOverview.RemindsWhenPending is ReminderOptionsWhenUserPending.RemindsDaily or ReminderOptionsWhenUserPending.RemindsDailyAndAheadOfTime);
                return (Reminder: dailyReminder, Events: eventsToRemind);
            });
        Task[] dailyReminderTasks = await dailyReminders
            .Select(r => reminderPublisher.PublishDailyReminderForUserAsync(r.Reminder.UserId, r.Reminder.Destination, r.Events, cancellationToken).AsTask())
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        var aotReminders = eventsToday.ToAsyncEnumerable()
            .SelectMany(async (e, ct) =>
            {
                var aotReminders = await userReminderRepository.GetUserAotRemindersAsync(e.StartsAt - utcTimeMinute, ct).ConfigureAwait(false);
                return aotReminders.Select(r => (AotReminder: r, Event: e));
            })
            .GroupBy(x => x.AotReminder.ReminderId)
            .Select(async (g, ct) =>
            {
                var aotReminder = g.First().AotReminder;
                var reminderOverview = await userReminderRepository.GetUserReminderOverviewAsync(aotReminder.ReminderId, ct).ConfigureAwait(false);
                var eventsToRemind = FilterEvents(
                    g.Select(x => x.Event),
                    aotReminder.UserId,
                    reminderOverview.RemindsOpenEvents is ReminderOptionsForOpenEvents.RemindsDailyAndAheadOfTime,
                    reminderOverview.RemindsWhenAbsent is ReminderOptionsWhenUserAbsent.RemindsDailyAndAheadOfTime,
                    reminderOverview.RemindsWhenPending is ReminderOptionsWhenUserPending.RemindsDailyAndAheadOfTime);
                return (Reminder: aotReminder, Events: eventsToRemind);
            });
        Task[] aotReminderTasks = await aotReminders
            .Select(r => reminderPublisher.PublishAotReminderForUserAsync(r.Reminder.UserId, r.Reminder.Destination, r.Events, cancellationToken).AsTask())
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        await Task.WhenAll([.. dailyReminderTasks, .. aotReminderTasks]).ConfigureAwait(false);
    }

    static ScheduledEvent[] FilterEvents(
        IEnumerable<ScheduledEvent> events,
        Guid userId,
        bool acceptOpenEvents,
        bool acceptWhenAbsent,
        bool acceptWhenPending)
    {
        return [.. events.Where(e =>
        {
            if (e.Attendees.TryGetValue(userId, out var stat))
            {
                return stat switch
                {
                    EventAttendanceStatus.Attending => true,
                    EventAttendanceStatus.Absent => acceptWhenAbsent,
                    EventAttendanceStatus.Pending => acceptWhenPending,
                    _ => false
                };
            }
            return e.IsOpen && acceptOpenEvents;
        })];
    }

    static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Warning, Message = "Reminder task time out occurred: running over {timeout}")]
        public static partial void LogWarning_ReminderTimeOut(ILogger logger, TimeSpan timeout);
    }
}

file static class UserReminderRepositoryExtensions
{
    public static async ValueTask<UserAotReminder[]> GetUserAotRemindersAsync(this IUserReminderRepository repo, TimeSpan offset, CancellationToken cancellationToken = default)
    {
        AheadOfTimeReminderTime aotReminderOffset = new(offset - TimeSpan.FromTicks(offset.Ticks % TimeSpan.TicksPerMinute));
        return await repo.GetUserAotRemindersAsync(aotReminderOffset, aotReminderOffset, cancellationToken).ConfigureAwait(false);
    }

    public static async ValueTask<UserDailyReminder[]> GetUserDailyRemindersAsync(this IUserReminderRepository repo, TimeOnly utcTimeOnly, CancellationToken cancellationToken = default)
    {
        var utcTimeMinutes = utcTimeOnly.Add(-TimeSpan.FromTicks(utcTimeOnly.Ticks % TimeSpan.TicksPerMinute));
        DailyReminderTime utcDailyReminderTime = new(utcTimeMinutes);
        return await repo.GetUserDailyRemindersAsync(utcDailyReminderTime, utcDailyReminderTime, cancellationToken).ConfigureAwait(false);
    }
}
