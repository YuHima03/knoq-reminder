namespace KnoqReminder.Domain.Services.Events;

public interface IEventProvider
{
    ValueTask<ScheduledEvent[]> GetEventsAsync(DateTimeOffset timeFrom, DateTimeOffset timeTo, CancellationToken cancellationToken = default);
}
