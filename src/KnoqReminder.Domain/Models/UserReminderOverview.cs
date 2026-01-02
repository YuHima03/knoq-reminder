namespace KnoqReminder.Domain.Models;

public sealed record UserReminderOverview(
    Guid Id,
    Guid UserId,
    ReminderOptionsWhenUserPending RemindsWhenPending,
    ReminderOptionsWhenUserAbsent RemindsWhenAbsent,
    ReminderOptionsForOpenEvents RemindsOpenEvents,
    DateTimeOffset UpdatedAt);
