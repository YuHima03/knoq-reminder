using KnoqReminder.Domain.Services.Events;

namespace KnoqReminder.Domain.Services.Reminder;

public interface IReminderPublisher
{
    ValueTask PublishAotReminderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default);

    ValueTask PublishDailyRemainderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default);
}
