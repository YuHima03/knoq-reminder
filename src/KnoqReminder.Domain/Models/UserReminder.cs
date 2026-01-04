namespace KnoqReminder.Domain.Models;

/// <summary>
/// Represents a setting for event reminders to a user.
/// </summary>
/// <param name="Id"></param>
/// <param name="UserId"></param>
/// <param name="RemindsWhenAbsent"></param>
/// <param name="RemindsFreeEvents"></param>
/// <param name="AheadOfTimeReminderTimes"></param>
/// <param name="DailyReminderTimes"></param>
/// <param name="CreatedAt"></param>
/// <param name="UpdatedAt"></param>
public record UserReminder(
    Guid Id,
    Guid UserId,
    ReminderOptionsWhenUserPending RemindsWhenPending,
    ReminderOptionsWhenUserAbsent RemindsWhenAbsent,
    ReminderOptionsForOpenEvents RemindsOpenEvents,
    AheadOfTimeReminderTime[] AheadOfTimeReminderTimes,
    DailyReminderTime[] DailyReminderTimes,
    DestinationDiscordWebhook[] DestinationDiscordWebhooks,
    DestinationTraqChannel[] DestinationTraqChannels,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
    );
