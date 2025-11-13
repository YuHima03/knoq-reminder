namespace KnoqReminder.Domain.Services.Events;

public interface IEventProvider
{
    ValueTask<ScheduledEvent?> GetScheduleAsync(Guid id, CancellationToken cancellationToken = default);
}
