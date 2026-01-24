using KnoqReminder.Domain.Models;
using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Repositories;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Reminder;
using Microsoft.Extensions.Options;
using ZLinq;

namespace KnoqReminder.App.Services.Reminder;

sealed partial class ReminderScheduler(
    IOptions<IReminderOptions> options,
    IEventProvider eventProvider,
    ILogger<ReminderScheduler> logger,
    IReminderPublisher reminderPublisher,
    IRepositoryProvider repositories
    )
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lastRunAt = DateTimeOffset.MinValue;
        await Task.Delay(options.Value.FirstRemindingTaskDelay, stoppingToken);
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(20));
        do
        {
            var utcNow = DateTimeOffset.UtcNow;
            if (lastRunAt.AddMinutes(1) <= utcNow) // Runs every minute
            {
                _ = Task.Run(async () =>
                {
                    using CancellationTokenSource cts = new(options.Value.RemindingTaskTimeout);
                    var ct = cts.Token;
                    try
                    {
                        await ExecuteCoreAsync(GetUtcTimeMinute(lastRunAt).AddMinutes(1), utcNow).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested)
                    {
                        LoggerExtensions.LogWarning_ReminderTimeOut(logger, options.Value.RemindingTaskTimeout);
                    }
                    catch (Exception ex)
                    {
                        LoggerExtensions.LogError_ReminderSchedulerError(logger, ex);
                    }
                }, CancellationToken.None);
                lastRunAt = utcNow;
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }

    async Task ExecuteCoreAsync(DateTimeOffset timeFrom, DateTimeOffset timeTo, CancellationToken cancellationToken = default)
    {
        timeFrom = GetUtcTimeMinute(timeFrom);
        timeTo = GetUtcTimeMinute(timeTo);

        var eventsToday = await eventProvider.GetEventsByStartTimeAsync(timeFrom, timeTo.AddDays(1), cancellationToken).ConfigureAwait(false);
        await using var repo = await repositories.CreateRepositoryAsync<IUserReminderRepository>(cancellationToken).ConfigureAwait(false);

        DailyReminderTime drtFrom = new(TimeOnly.FromDateTime(timeFrom.UtcDateTime));
        DailyReminderTime drtTo = new(TimeOnly.FromDateTime(timeTo.UtcDateTime));
        var dailyReminders = (await repo.GetUserDailyRemindersAsync(drtFrom, drtTo, cancellationToken).ConfigureAwait(false))
            .ToAsyncEnumerable()
            .Select(async (dailyReminder, ct) =>
            {
                var reminderOverview = await repo.GetUserReminderOverviewAsync(dailyReminder.ReminderId, ct).ConfigureAwait(false);
                var eventsToRemind = FilterEvents(
                    eventsToday,
                    dailyReminder.UserId,
                    reminderOverview.RemindsOpenEvents is ReminderOptionsForOpenEvents.RemindsDaily or ReminderOptionsForOpenEvents.RemindsDailyAndAheadOfTime,
                    reminderOverview.RemindsWhenAbsent is ReminderOptionsWhenUserAbsent.RemindsDaily or ReminderOptionsWhenUserAbsent.RemindsDailyAndAheadOfTime,
                    reminderOverview.RemindsWhenPending is ReminderOptionsWhenUserPending.RemindsDaily or ReminderOptionsWhenUserPending.RemindsDailyAndAheadOfTime);
                return (Reminder: dailyReminder, Events: eventsToRemind);
            })
            .Where(x => x.Events is not []);
        Task[] dailyReminderTasks = await dailyReminders
            .Select(r => reminderPublisher.PublishDailyReminderForUserAsync(r.Reminder.UserId, r.Reminder.Destination, r.Events, cancellationToken).AsTask())
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        var aotReminders = eventsToday.ToAsyncEnumerable()
            .SelectMany(async (e, ct) =>
            {
                var offsetFrom = e.StartsAt - timeTo;
                var offsetTo = e.StartsAt - timeFrom;
                var aotReminders = await repo.GetUserAotRemindersAsync(new(offsetFrom), new(offsetTo), ct).ConfigureAwait(false);
                return aotReminders.Select(r => (AotReminder: r, Event: e));
            })
            .GroupBy(x => x.AotReminder.ReminderId)
            .Select(async (g, ct) =>
            {
                var aotReminder = g.First().AotReminder;
                var reminderOverview = await repo.GetUserReminderOverviewAsync(aotReminder.ReminderId, ct).ConfigureAwait(false);
                var eventsToRemind = FilterEvents(
                    g.Select(x => x.Event),
                    aotReminder.UserId,
                    reminderOverview.RemindsOpenEvents is ReminderOptionsForOpenEvents.RemindsDailyAndAheadOfTime,
                    reminderOverview.RemindsWhenAbsent is ReminderOptionsWhenUserAbsent.RemindsDailyAndAheadOfTime,
                    reminderOverview.RemindsWhenPending is ReminderOptionsWhenUserPending.RemindsDailyAndAheadOfTime);
                return (Reminder: aotReminder, Events: eventsToRemind);
            })
            .Where(x => x.Events is not []);
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

    static DateTimeOffset GetUtcTimeMinute(DateTimeOffset dto)
    {
        dto = dto.ToUniversalTime();
        return dto - TimeSpan.FromTicks(dto.Ticks % TimeSpan.TicksPerMinute);
    }

    static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while executing reminder scheduler.")]
        public static partial void LogError_ReminderSchedulerError(ILogger<ReminderScheduler> logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Reminder task time out occurred: running over {timeout}")]
        public static partial void LogWarning_ReminderTimeOut(ILogger<ReminderScheduler> logger, TimeSpan timeout);
    }
}
