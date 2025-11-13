using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Reminder;
using Traq;

namespace KnoqReminder.App.Services.Reminder;

public class ReminderPublisherImplement(
    TraqApiClient traq,
    ILogger<ReminderPublisherImplement> logger
    )
    : IReminderPublisher
{
    public ValueTask PublishAotReminderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask PublishDailyRemainderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
