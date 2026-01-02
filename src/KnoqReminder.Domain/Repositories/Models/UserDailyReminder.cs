using KnoqReminder.Domain.Services.Reminder;

namespace KnoqReminder.Domain.Repositories.Models;

public record struct UserDailyReminder(
    Guid ReminderId,
    Guid UserId,
    ReminderDestination Destination,
    DailyReminderTime RemindsAt);
