namespace KnoqReminder.Domain.Models;

public record UserReminderAddOrUpdateRequest(
    Guid? Id,
    Guid UserId,
    ReminderKind RemindsWhenAbsent,
    ReminderKind RemindsFreeEvents,
    AheadOfTimeReminderTime[] AheadOfTimeReminderTimes,
    DailyReminderTime[] DailyReminderTimes
    );
