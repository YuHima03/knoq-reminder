using System.Linq.Expressions;
using KnoqReminder.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DestinationTraqChannelHelper
{
    public static readonly Expression<Func<DestinationTraqChannel, Domain.Models.DestinationTraqChannel>> DtoToDomainExpression = x => new Domain.Models.DestinationTraqChannel
    {
        ChannelId = x.ChannelId,
    };

    public static IQueryable<Domain.Models.UserReminder> GroupJoinTraqChannels(this IQueryable<Domain.Models.UserReminder> @this, DbSet<DestinationTraqChannel> destinationsTraqs)
    {
        return @this.GroupJoin(
            destinationsTraqs.AsNoTracking(),
            ur => ur.Id,
            tc => tc.ReminderId,
            (ur, tcs) => new Domain.Models.UserReminder(
                Id: ur.Id,
                UserId: ur.UserId,
                RemindsWhenPending: ur.RemindsWhenPending,
                RemindsWhenAbsent: ur.RemindsWhenAbsent,
                RemindsOpenEvents: ur.RemindsOpenEvents,
                AheadOfTimeReminderTimes: ur.AheadOfTimeReminderTimes,
                DailyReminderTimes: ur.DailyReminderTimes,
                DestinationDiscordWebhooks: ur.DestinationDiscordWebhooks,
                DestinationTraqChannels: tcs.Select(ToDomain).ToArray(),
                CreatedAt: ur.CreatedAt,
                UpdatedAt: ur.UpdatedAt
            )
        );
    }

    public static Domain.Models.DestinationTraqChannel ToDomain(this DestinationTraqChannel dto)
    {
        return new()
        {
            ChannelId = dto.ChannelId,
        };
    }
}
