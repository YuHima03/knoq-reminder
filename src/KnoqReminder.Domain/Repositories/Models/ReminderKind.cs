namespace KnoqReminder.Domain.Repositories.Models;

/// <summary>
/// Represents which kinds of reminders to send.
/// </summary>
public enum ReminderKind
{
    /// <summary>
    /// Does not send reminders.
    /// </summary>
    None = 0,

    /// <summary>
    /// Sends daily reminders only.
    /// </summary>
    Daily = 1,

    /// <summary>
    /// Sends all reminders including daily and ahead-of-time reminders.
    /// </summary>
    Always = 2
}
