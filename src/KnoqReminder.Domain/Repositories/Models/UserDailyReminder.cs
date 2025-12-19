using KnoqReminder.Domain.Services.Reminder;

namespace KnoqReminder.Domain.Repositories.Models;

public record struct UserDailyReminder(
    Guid ReminderId,
    ReminderDestination Destination,
    DailyReminderTime RemindsAt);
