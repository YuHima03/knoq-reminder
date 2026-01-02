using System;
using System.Collections.Generic;
using System.Text;
using KnoqReminder.Domain.Repositories.Models;

namespace KnoqReminder.Domain.Reminder;

public sealed record UserReminderOverview(
    Guid Id,
    Guid UserId,
    ReminderKind RemindsWhenAbsent,
    ReminderKind RemindsFreeEvents,
    DateTimeOffset UpdatedAt);
