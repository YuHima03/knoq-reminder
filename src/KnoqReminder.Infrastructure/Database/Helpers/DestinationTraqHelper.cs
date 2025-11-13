using System.Linq.Expressions;
using KnoqReminder.Domain.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DestinationTraqHelper
{
    public static readonly Expression<Func<DestinationsTraq, DestinationTraqChannel>> DtoToDomainExpression = x => new DestinationTraqChannel
    {
        ChannelId = x.ChannelId,
    };

    public static IQueryable<Domain.Repositories.Models.UserReminder> GroupJoinTraqChannels(this IQueryable<Domain.Repositories.Models.UserReminder> @this, DbSet<DestinationsTraq> destinationsTraqs)
    {
        return @this.GroupJoin(
            destinationsTraqs.AsNoTracking(),
            ur => ur.Id,
            tc => tc.ReminderId,
            (ur, tcs) => new Domain.Repositories.Models.UserReminder(
                Id: ur.Id,
                UserId: ur.UserId,
                RemindsWhenAbsent: ur.RemindsWhenAbsent,
                RemindsFreeEvents: ur.RemindsFreeEvents,
                AheadOfTimeReminderTimes: ur.AheadOfTimeReminderTimes,
                DailyReminderTimes: ur.DailyReminderTimes,
                DestinationDiscordWebhooks: ur.DestinationDiscordWebhooks,
                DestinationTraqChannels: tcs.Select(ToDomain).ToArray(),
                CreatedAt: ur.CreatedAt,
                UpdatedAt: ur.UpdatedAt
            )
        );
    }

    public static DestinationTraqChannel ToDomain(this DestinationsTraq dto)
    {
        return new()
        {
            ChannelId = dto.ChannelId,
        };
    }
}
