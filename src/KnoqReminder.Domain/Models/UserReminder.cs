namespace KnoqReminder.Domain.Models;

public record UserReminder(
    Guid Id,
    Guid UserId,
    ReminderKind RemindsWhenAbsent,
    ReminderKind RemindsFreeEvents,
    TimeSpan[] AheadOfTimeReminderTimes,
    TimeOnly[] DailyReminderTimes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
    );
