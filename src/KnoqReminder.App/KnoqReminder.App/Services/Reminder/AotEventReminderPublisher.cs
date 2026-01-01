using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Reminder;

namespace KnoqReminder.App.Services.Reminder;

sealed partial class ReminderPublisher
{
    public ValueTask PublishAotReminderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
