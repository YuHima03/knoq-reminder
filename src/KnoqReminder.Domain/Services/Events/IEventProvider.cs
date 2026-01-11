namespace KnoqReminder.Domain.Services.Events;

public interface IEventProvider
{
    ValueTask<ScheduledEvent[]> GetEventsByStartTimeAsync(DateTimeOffset startTimeFrom, DateTimeOffset startTimeTo, CancellationToken cancellationToken = default);
}
