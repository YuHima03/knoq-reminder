namespace KnoqReminder.Domain.Services.Events;

public record ScheduledEvent(
    Guid Id,
    string Name,
    string Place,
    Guid HostGroupId,
    string Description,
    bool IsOpen,
    Dictionary<Guid, EventAttendanceStatus> Attendees,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt
    );
