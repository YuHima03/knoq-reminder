using CommunityToolkit.Diagnostics;
using KnoqReminder.Domain.Models;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static partial class UserReminderHelper
{
    public static IQueryable<UserReminderOverview> SelectDomainUserReminderOverview(this IQueryable<UserReminder> dtoQueryable)
    {
        return dtoQueryable.Select(dto => new UserReminderOverview(
            dto.Id,
            dto.UserId,
            ParseDtoStringToReminderOptionsWhenUserPending(dto.RemindsWhenPending),
            ParseDtoStringToReminderOptionsWhenUserAbsent(dto.RemindsWhenAbsent),
            ParseDtoStringToReminderOptionsForOpenEvents(dto.RemindsFreeEvents),
            dto.UpdatedAt));
    }

    public static IQueryable<Domain.Models.UserReminder> SelectDomainUserReminder(this IQueryable<UserReminder> dtoQueryable)
    {
        return dtoQueryable.Select(dto => new Domain.Models.UserReminder(
            dto.Id,
            dto.UserId,
            ParseDtoStringToReminderOptionsWhenUserPending(dto.RemindsWhenPending),
            ParseDtoStringToReminderOptionsWhenUserAbsent(dto.RemindsWhenAbsent),
            ParseDtoStringToReminderOptionsForOpenEvents(dto.RemindsFreeEvents),
            dto.AheadOfTimeReminders
                .AsQueryable()
                .Select(ar => new AheadOfTimeReminderTime(ar.Offset))
                .ToArray(),
            dto.DailyReminders
                .AsQueryable()
                .Select(dr => new DailyReminderTime(TimeOnly.FromTimeSpan(dr.Time)))
                .ToArray(),
            dto.DestinationDiscordWebhooks
                .AsQueryable()
                .SelectDomainDestinationDiscordWebhook()
                .ToArray(),
            dto.DestinationTraqChannels
                .AsQueryable()
                .SelectDomainDestinationTraqChannel()
                .ToArray(),
            dto.CreatedAt,
            dto.UpdatedAt));
    }
}

static partial class UserReminderHelper
{
    public static ReminderOptionsForOpenEvents ParseDtoStringToReminderOptionsForOpenEvents(string value)
    {
        if (value.Equals("none", StringComparison.InvariantCultureIgnoreCase))
        {
            return ReminderOptionsForOpenEvents.None;
        }
        else if (value.Equals("daily", StringComparison.InvariantCultureIgnoreCase))
        {
            return ReminderOptionsForOpenEvents.RemindsDaily;
        }
        else if (value.Equals("always", StringComparison.InvariantCultureIgnoreCase))
        {
            return ReminderOptionsForOpenEvents.RemindsDailyAndAheadOfTime;
        }
        return ThrowHelper.ThrowArgumentException<ReminderOptionsForOpenEvents>("Unknown string value for enum");
    }

    public static ReminderOptionsWhenUserAbsent ParseDtoStringToReminderOptionsWhenUserAbsent(string value)
    {
        if (value.Equals("none", StringComparison.InvariantCultureIgnoreCase))
        {
            return ReminderOptionsWhenUserAbsent.None;
        }
        else if (value.Equals("daily", StringComparison.InvariantCultureIgnoreCase))
        {
            return ReminderOptionsWhenUserAbsent.RemindsDaily;
        }
        else if (value.Equals("always", StringComparison.InvariantCultureIgnoreCase))
        {
            return ReminderOptionsWhenUserAbsent.RemindsDailyAndAheadOfTime;
        }
        return ThrowHelper.ThrowArgumentException<ReminderOptionsWhenUserAbsent>("Unknown string value for enum");
    }

    public static ReminderOptionsWhenUserPending ParseDtoStringToReminderOptionsWhenUserPending(string value)
    {
        if (value.Equals("daily", StringComparison.InvariantCultureIgnoreCase))
        {
            return ReminderOptionsWhenUserPending.RemindsDaily;
        }
        else if (value.Equals("always", StringComparison.InvariantCultureIgnoreCase))
        {
            return ReminderOptionsWhenUserPending.RemindsDailyAndAheadOfTime;
        }
        return ThrowHelper.ThrowArgumentException<ReminderOptionsWhenUserPending>("Unknown string value for enum");
    }

    public static string ToDtoString(this ReminderOptionsForOpenEvents @enum) => @enum switch
    {
        ReminderOptionsForOpenEvents.None => "none",
        ReminderOptionsForOpenEvents.RemindsDaily => "daily",
        ReminderOptionsForOpenEvents.RemindsDailyAndAheadOfTime => "always",
        _ => ""
    };

    public static string ToDtoString(this ReminderOptionsWhenUserAbsent @enum) => @enum switch
    {
        ReminderOptionsWhenUserAbsent.None => "none",
        ReminderOptionsWhenUserAbsent.RemindsDaily => "daily",
        ReminderOptionsWhenUserAbsent.RemindsDailyAndAheadOfTime => "always",
        _ => ""
    };

    public static string ToDtoString(this ReminderOptionsWhenUserPending @enum) => @enum switch
    {
        ReminderOptionsWhenUserPending.RemindsDaily => "daily",
        ReminderOptionsWhenUserPending.RemindsDailyAndAheadOfTime => "always",
        _ => ""
    };
}
