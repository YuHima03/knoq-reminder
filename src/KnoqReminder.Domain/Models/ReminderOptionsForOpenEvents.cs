namespace KnoqReminder.Domain.Models;

/// <summary>
/// Indicates which type of reminder to send for open events.
/// </summary>
public enum ReminderOptionsForOpenEvents
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
