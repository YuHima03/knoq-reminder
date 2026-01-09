using System.Linq.Expressions;
using KnoqReminder.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DestinationDiscordWebhookHelper
{
    public static readonly Expression<Func<DestinationDiscordWebhook, Domain.Models.DestinationDiscordWebhook>> DtoToDomainExpression = x => new Domain.Models.DestinationDiscordWebhook
    {
        WebhookId = x.WebhookId,
        WebhookSecret = x.WebhookSecret,
    };

    public static IQueryable<Domain.Models.UserReminder> GroupJoinDiscordWebhooks(this IQueryable<Domain.Models.UserReminder> @this, DbSet<DestinationDiscordWebhook> destinationsDiscords)
    {
        return @this.GroupJoin(
            destinationsDiscords.AsNoTracking(),
            ur => ur.Id,
            dw => dw.ReminderId,
            (ur, dws) => new Domain.Models.UserReminder(
                Id: ur.Id,
                UserId: ur.UserId,
                RemindsWhenPending: ur.RemindsWhenPending,
                RemindsWhenAbsent: ur.RemindsWhenAbsent,
                RemindsOpenEvents: ur.RemindsOpenEvents,
                AheadOfTimeReminderTimes: ur.AheadOfTimeReminderTimes,
                DailyReminderTimes: ur.DailyReminderTimes,
                DestinationDiscordWebhooks: dws.Select(ToDomain).ToArray(),
                DestinationTraqChannels: ur.DestinationTraqChannels,
                CreatedAt: ur.CreatedAt,
                UpdatedAt: ur.UpdatedAt
            )
        );
    }

    public static Domain.Models.DestinationDiscordWebhook ToDomain(this DestinationDiscordWebhook dto)
    {
        return new()
        {
            WebhookId = dto.WebhookId,
            WebhookSecret = dto.WebhookSecret,
        };
    }
}
