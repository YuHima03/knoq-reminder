using KnoqReminder.Domain.Services.Reminder;

namespace KnoqReminder.Domain.Repositories.Models;

public record struct UserAotReminder(
    Guid ReminderId,
    Guid UserId,
    ReminderDestination Destination,
    AheadOfTimeReminderTime Offset);
