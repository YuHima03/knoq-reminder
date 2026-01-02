using System.Linq.Expressions;
using KnoqReminder.Domain.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DestinationDiscordHelper
{
    public static Expression<Func<DestinationsDiscord, DestinationDiscordWebhook>> DtoToDomainExpression = x => new DestinationDiscordWebhook
    {
        WebhookId = x.WebhookId,
        WebhookSecret = x.WebhookSecret,
    };

    public static IQueryable<Domain.Reminder.Models.UserReminder> GroupJoinDiscordWebhooks(this IQueryable<Domain.Reminder.Models.UserReminder> @this, DbSet<DestinationsDiscord> destinationsDiscords)
    {
        return @this.GroupJoin(
            destinationsDiscords.AsNoTracking(),
            ur => ur.Id,
            dw => dw.ReminderId,
            (ur, dws) => new Domain.Repositories.Models.UserReminder(
                Id: ur.Id,
                UserId: ur.UserId,
                RemindsWhenAbsent: ur.RemindsWhenAbsent,
                RemindsFreeEvents: ur.RemindsFreeEvents,
                AheadOfTimeReminderTimes: ur.AheadOfTimeReminderTimes,
                DailyReminderTimes: ur.DailyReminderTimes,
                DestinationDiscordWebhooks: dws.Select(ToDomain).ToArray(),
                DestinationTraqChannels: ur.DestinationTraqChannels,
                CreatedAt: ur.CreatedAt,
                UpdatedAt: ur.UpdatedAt
            )
        );
    }

    public static DestinationDiscordWebhook ToDomain(this DestinationsDiscord dto)
    {
        return new()
        {
            WebhookId = dto.WebhookId,
            WebhookSecret = dto.WebhookSecret,
        };
    }
}
