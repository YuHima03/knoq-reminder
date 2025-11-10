namespace KnoqReminder.Domain.Models;

public record UserReminder(
    Guid Id,
    Guid UserId,
    ReminderKind RemindsWhenAbsent,
    ReminderKind RemindsFreeEvents,
    AheadOfTimeReminderTime[] AheadOfTimeReminderTimes,
    DailyReminderTime[] DailyReminderTimes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
    );
