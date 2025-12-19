using KnoqReminder.Domain.Services.Reminder;

namespace KnoqReminder.Domain.Repositories.Models;

public record struct UserAotReminder(
    Guid ReminderId,
    ReminderDestination Destination,
    AheadOfTimeReminderTime Offset);
