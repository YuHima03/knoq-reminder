using System.Linq.Expressions;
using KnoqReminder.Domain.Models;

namespace KnoqReminder.Infrastructure.Database.Converters;

static class UserReminderConverter
{
    public static readonly Expression<Func<UserReminder, Domain.Models.UserReminder>> DtoToDomainExpression = dto => new(
        dto.Id,
        dto.UserId,
        Enum.Parse<ReminderKind>(dto.RemindsWhenAbsent, true),
        Enum.Parse<ReminderKind>(dto.RemindsFreeEvents, true),
        dto.AheadOfTimeReminders.Select(ar => new AheadOfTimeReminderTime(ar.Duration)).ToArray(),
        dto.DailyReminders.Select(dr => new DailyReminderTime(TimeOnly.FromTimeSpan(dr.Time))).ToArray(),
        dto.CreatedAt,
        dto.UpdatedAt);
}
