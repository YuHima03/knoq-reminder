namespace KnoqReminder.Domain.Models;

public record struct UserDailyReminder(
    Guid ReminderId,
    Guid UserId,
    ReminderDestination Destination,
    DailyReminderTime RemindsAt);
