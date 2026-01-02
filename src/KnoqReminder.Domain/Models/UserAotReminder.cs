namespace KnoqReminder.Domain.Models;

public record struct UserAotReminder(
    Guid ReminderId,
    Guid UserId,
    ReminderDestination Destination,
    AheadOfTimeReminderTime Offset);
