using KnoqReminder.Domain.Repositories.Models;

namespace KnoqReminder.Domain.Reminder.Models;

/// <summary>
/// Represents a configuration for adding a new item to or updating an existing item in a set of <see cref="UserReminder"/>s.
/// </summary>
/// <param name="UserId"></param>
/// <param name="RemindsWhenAbsent"></param>
/// <param name="RemindsFreeEvents"></param>
/// <param name="AheadOfTimeReminderTimes"></param>
/// <param name="DailyReminderTimes"></param>
public record UserReminderAddOrUpdateRequest(
    Guid? UserId,
    ReminderOptionsWhenUserPending? RemindsWhenPending,
    ReminderOptionsWhenUserAbsent? RemindsWhenAbsent,
    ReminderOptionsForOpenEvents? RemindsOpenEvents,
    AheadOfTimeReminderTime[]? AheadOfTimeReminderTimes,
    DailyReminderTime[]? DailyReminderTimes,
    DestinationDiscordWebhook[]? DestinationDiscordWebhooks,
    DestinationTraqChannel[]? DestinationTraqChannels
    );
