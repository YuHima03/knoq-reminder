using System.Linq.Expressions;
using KnoqReminder.Domain.Models;
using KnoqReminder.Utilities.Linq;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class ReminderDestinationHelper
{
    public static IQueryable<DefaultJoinResult<TSource, ReminderDestination>> GroupJoinDestinations<TSource>(
        this IQueryable<TSource> @this,
        IQueryable<DestinationsDiscord> destinationDiscordSet,
        IQueryable<DestinationsTraq> destinationTraqSet,
        Expression<Func<TSource, Guid>> outerReminderIdSelector,
        Expression<Func<(TSource, IEnumerable<DestinationsDiscord>), Guid>> outerReminderIdSelector2)
    {
        return @this
            .GroupJoin(
                inner: destinationDiscordSet,
                outerKeySelector: outerReminderIdSelector,
                innerKeySelector: dest => dest.ReminderId,
                resultSelector: (reminders, discordWebhooks) => ValueTuple.Create(reminders, discordWebhooks))
            .GroupJoin(
                inner: destinationTraqSet,
                outerKeySelector: outerReminderIdSelector2,
                innerKeySelector: dest => dest.ReminderId,
                resultSelector: (t, traqChannels) => DefaultJoinResult.Create(
                    t.Item1,
                    new ReminderDestination
                    {
                        DiscordWebhooks = t.Item2.Select(DestinationDiscordHelper.ToDomain).ToArray(),
                        TraqChannels = traqChannels.Select(DestinationTraqHelper.ToDomain).ToArray()
                    }));
    }

    public static IQueryable<DefaultJoinResult<IGrouping<Guid, TSource>, ReminderDestination>> GroupJoinDestinations<TSource>(
        this IQueryable<IGrouping<Guid, TSource>> @this,
        IQueryable<DestinationsDiscord> destinationDiscordSet,
        IQueryable<DestinationsTraq> destinationTraqSet)
    {
        return @this.GroupJoinDestinations(
            destinationDiscordSet,
            destinationTraqSet,
            g => g.Key,
            t => t.Item1.Key);
    }
}
