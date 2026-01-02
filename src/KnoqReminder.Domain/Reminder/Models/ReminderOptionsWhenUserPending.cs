using System;
using System.Collections.Generic;
using System.Text;

namespace KnoqReminder.Domain.Reminder.Models;

/// <summary>
/// Indicates which type of reminder to send for events which the user does not decide attendance yet.
/// </summary>
public enum ReminderOptionsWhenUserPending
{
    /// <summary>
    /// Sends daily reminders only.
    /// </summary>
    RemindsDaily = 0,

    /// <summary>
    /// Sends all reminders including daily and ahead-of-time reminders.
    /// </summary>
    RemindsDailyAndAheadOfTime
}
