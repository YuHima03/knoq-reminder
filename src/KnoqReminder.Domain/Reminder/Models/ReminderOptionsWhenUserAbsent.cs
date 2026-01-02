namespace KnoqReminder.Domain.Reminder.Models;

/// <summary>
/// Indicates which type of reminder to send for events when the user is absent.
/// </summary>
public enum ReminderOptionsWhenUserAbsent
{
    /// <summary>
    /// Does not send reminders.
    /// </summary>
    None = 0,

    /// <summary>
    /// Sends daily reminders only.
    /// </summary>
    RemindsDaily,

    /// <summary>
    /// Sends all reminders including daily and ahead-of-time reminders.
    /// </summary>
    RemindsDailyAndAheadOfTime
}
