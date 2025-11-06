using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnoqReminder.Domain.Models;

public record UserReminder(
    Guid Id,
    Guid UserId,
    ReminderKind RemindsWhenAbsent,
    ReminderKind RemindsFreeEvents,
    TimeSpan[] AheadOfTimeReminderTimes,
    TimeOnly[] DailyReminderTimes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
    );
