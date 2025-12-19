using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using KnoqReminder.Domain.Services.Reminder;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class ReminderDestinationHelper
{
    public static IQueryable<(TSource, ReminderDestination)> GroupJoinDestinations<TSource>(
        this IQueryable<TSource> @this,
        IQueryable<DestinationsDiscord> destinationDiscordSet,
        IQueryable<DestinationsTraq> destinationTraqSet,
        Expression<Func<TSource, Guid>> outerReminderIdSelector)
    {
        return @this
            .GroupJoin(
                inner: destinationDiscordSet,
                outerKeySelector: outerReminderIdSelector,
                innerKeySelector: dest => dest.ReminderId,
                resultSelector: (reminders, discordWebhooks) => ValueTuple.Create(reminders, discordWebhooks))
            .GroupJoin(
                inner: destinationTraqSet,
                outerKeySelector: createOuterKeySelectorFor2ndGroupJoin(outerReminderIdSelector),
                innerKeySelector: dest => dest.ReminderId,
                resultSelector: (t, traqChannels) => ValueTuple.Create(
                    item1: t.Item1,
                    item2: new ReminderDestination
                    {
                        DiscordWebhooks = t.Item2.Select(DestinationDiscordHelper.ToDomain).ToArray(),
                        TraqChannels = traqChannels.Select(DestinationTraqHelper.ToDomain).ToArray()
                    }));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Expression<Func<(TSource, IEnumerable<DestinationsDiscord>), Guid>> createOuterKeySelectorFor2ndGroupJoin<TSource>(Expression<Func<TSource, Guid>> selector)
        {
            var param0 = Expression.Parameter(typeof((TSource, IEnumerable<DestinationsDiscord>)));
            return Expression.Lambda<Func<(TSource, IEnumerable<DestinationsDiscord>), Guid>>(
                tailCall: false,
                parameters: [Expression.Parameter(typeof((TSource, IEnumerable<DestinationsDiscord>)))],
                body: Expression.Invoke(selector, param0));
        }
    }
}
