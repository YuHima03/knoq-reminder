using System.Buffers;
using KnoqReminder.Domain.Exceptions;
using KnoqReminder.Domain.Models;
using KnoqReminder.Domain.Repositories;
using KnoqReminder.Infrastructure.Database.Helpers;
using KnoqReminder.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using ZLinq;

namespace KnoqReminder.Infrastructure.Database;

public partial class AppDbContext : IUserReminderRepository
{
    async ValueTask<Domain.Models.UserReminder> IUserReminderRepository.AddUserReminderAsync(UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken)
    {
        var reminderId = Guid.CreateVersion7();
        UserReminder reminder = new()
        {
            Id = reminderId,
            UserId = item.UserId.GetValueOrDefault(),
            RemindsWhenPending = item.RemindsWhenPending.GetValueOrDefault().ToDtoString(),
            RemindsWhenAbsent = item.RemindsWhenAbsent.GetValueOrDefault().ToDtoString(),
            RemindsFreeEvents = item.RemindsOpenEvents.GetValueOrDefault().ToDtoString(),
            AheadOfTimeReminders = [.. (item.AheadOfTimeReminderTimes ?? [])
                .Distinct()
                .Select(x => new AheadOfTimeReminder
                {
                    Id = Guid.CreateVersion7(),
                    ReminderId = reminderId,
                    Offset = x.TimeSpan
                })],
            DailyReminders = [.. (item.DailyReminderTimes ?? [])
                .Distinct()
                .Select(x => new DailyReminder
                {
                    Id = Guid.CreateVersion7(),
                    ReminderId = reminderId,
                    Time = x.TimeSpan
                })],
            DestinationDiscordWebhooks = [.. (item.DestinationDiscordWebhooks ?? [])
                .DistinctBy(x => x.WebhookId)
                .Select(x => new DestinationDiscordWebhook
                {
                    Id = Guid.CreateVersion7(),
                    ReminderId = reminderId,
                    WebhookId = x.WebhookId,
                    WebhookSecret = x.WebhookSecret
                })],
            DestinationTraqChannels = [.. (item.DestinationTraqChannels ?? [])
                .DistinctBy(x => x.ChannelId)
                .Select(x => new DestinationTraqChannel
                {
                    Id = Guid.CreateVersion7(),
                    ReminderId = reminderId,
                    ChannelId = x.ChannelId
                })]
        };
        var entity = UserReminders.Add(reminder);
        await SaveChangesAsync(cancellationToken);
        return entity.Entity.ToDomainUserReminder();
    }

    async ValueTask IUserReminderRepository.DeleteUserRemindersAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var idsArray = ids.AsValueEnumerable().Distinct().ToArray();
        if (idsArray.Length == 0)
        {
            return;
        }
        else if (idsArray.Length == 1)
        {
            var id = idsArray[0];
            await UserReminders.AsNoTracking()
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
        }
        else
        {
            await UserReminders.AsNoTracking()
                .Where(x => idsArray.Contains(x.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }
    }

    async ValueTask<UserAotReminder[]> IUserReminderRepository.GetUserAotRemindersAsync(AheadOfTimeReminderTime offsetFrom, AheadOfTimeReminderTime offsetTo, CancellationToken cancellationToken)
    {
        var q = (offsetFrom == offsetTo)
            ? AheadOfTimeReminders.AsNoTracking().Where(x => x.Offset == offsetFrom.TimeSpan)
            : AheadOfTimeReminders.AsNoTracking().Where(x => offsetFrom.TimeSpan <= x.Offset && x.Offset <= offsetTo.TimeSpan);
        return await q
            // Note: Please ensure that `DestinationDiscordWebhooks.Count() * DestinationTraqChannels.Count()` is small enough to use AsSingleQuery() method.
            .AsSingleQuery()
            .OrderBy(r => r.Offset)
            .Select(ar => new UserAotReminder(
                ar.ReminderId,
                ar.Reminder.UserId,
                new ReminderDestination
                {
                    DiscordWebhooks = ar.Reminder.DestinationDiscordWebhooks
                        .SelectDomainDestinationDiscordWebhook()
                        .ToArray(),
                    TraqChannels = ar.Reminder.DestinationTraqChannels
                        .SelectDomainDestinationTraqChannel()
                        .ToArray()
                },
                new AheadOfTimeReminderTime(ar.Offset)))
            .ToArrayAsync(cancellationToken);
    }

    async ValueTask<UserDailyReminder[]> IUserReminderRepository.GetUserDailyRemindersAsync(DailyReminderTime timeFrom, DailyReminderTime timeTo, CancellationToken cancellationToken)
    {
        var q = (timeFrom == timeTo)
            ? DailyReminders.AsNoTracking().Where(x => x.Time == timeFrom.TimeSpan)
            : DailyReminders.AsNoTracking().Where(x => timeFrom.TimeSpan <= x.Time && x.Time <= timeTo.TimeSpan);
        return await q
            // Note: Please ensure that `DestinationDiscordWebhooks.Count() * DestinationTraqChannels.Count()` is small enough to use AsSingleQuery() method.
            .AsSingleQuery()
            .OrderBy(r => r.Time)
            .Select(dr => new UserDailyReminder(
                dr.ReminderId,
                dr.Reminder.UserId,
                new ReminderDestination
                {
                    DiscordWebhooks = dr.Reminder.DestinationDiscordWebhooks
                        .SelectDomainDestinationDiscordWebhook()
                        .ToArray(),
                    TraqChannels = dr.Reminder.DestinationTraqChannels
                        .SelectDomainDestinationTraqChannel()
                        .ToArray()
                },
                new DailyReminderTime(TimeOnly.FromTimeSpan(dr.Time))))
            .ToArrayAsync(cancellationToken);
    }

    async ValueTask<Domain.Models.UserReminder> IUserReminderRepository.GetUserReminderAsync(Guid id, CancellationToken cancellationToken)
    {
        return await UserReminders.AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Id == id)
            .SelectDomainUserReminder()
            .FirstOrDefaultAsync(cancellationToken)
            ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, Domain.Models.UserReminder>();
    }

    async ValueTask<Domain.Models.UserReminder> IUserReminderRepository.GetUserReminderByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await UserReminders.AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.UserId == userId)
            .SelectDomainUserReminder()
            .FirstOrDefaultAsync(cancellationToken)
            ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, Domain.Models.UserReminder>();
    }

    async ValueTask<UserReminderOverview> IUserReminderRepository.GetUserReminderOverviewAsync(Guid id, CancellationToken cancellationToken)
    {
        return await UserReminders.AsNoTracking()
            .Where(x => x.Id == id)
            .SelectDomainUserReminderOverview()
            .FirstOrDefaultAsync(cancellationToken)
            ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, UserReminderOverview>();
    }

    async ValueTask<Domain.Models.UserReminder> IUserReminderRepository.UpdateUserReminderAsync(Guid id, UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken)
    {
        var query = UserReminders
            .AsNoTracking()
            .Where(x => x.Id == id);
        var entity = await query
            .SingleOrDefaultAsync(cancellationToken)
            ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, UserReminder>();
        if (item.UserId is not null)
        {
            entity.UserId = item.UserId.Value;
        }
        if (item.RemindsWhenPending is not null)
        {
            entity.RemindsWhenPending = item.RemindsWhenPending.Value.ToDtoString();
        }
        if (item.RemindsWhenAbsent is not null)
        {
            entity.RemindsWhenAbsent = item.RemindsWhenAbsent.Value.ToDtoString();
        }
        if (item.RemindsOpenEvents is not null)
        {
            entity.RemindsFreeEvents = item.RemindsOpenEvents.Value.ToDtoString();
        }
        if (item.AheadOfTimeReminderTimes is not null)
        {
            using var prev = entity.AheadOfTimeReminders.AsValueEnumerable().ToArrayPool();
            AheadOfTimeReminders.ApplyChanges(
                item.AheadOfTimeReminderTimes,
                prev.Span,
                e => new AheadOfTimeReminderTime(e.Offset),
                v => new AheadOfTimeReminder { Id = Guid.CreateVersion7(), ReminderId = id, Offset = v.TimeSpan }
            );
        }
        if (item.DailyReminderTimes is not null)
        {
            using var prev = entity.DailyReminders.AsValueEnumerable().ToArrayPool();
            DailyReminders.ApplyChanges(
                item.DailyReminderTimes,
                prev.Span,
                e => new DailyReminderTime(TimeOnly.FromTimeSpan(e.Time)),
                v => new DailyReminder { Id = Guid.CreateVersion7(), ReminderId = id, Time = v.TimeSpan }
            );
        }
        if (item.DestinationDiscordWebhooks is not null)
        {
            var prev = await DestinationDiscordWebhooks.AsNoTracking()
                .Where(x => x.ReminderId == id)
                .ToArrayAsync(cancellationToken);
            DestinationDiscordWebhooks.ApplyChangesSlow(
                item.DestinationDiscordWebhooks,
                prev.AsSpan(),
                e => new Domain.Models.DestinationDiscordWebhook { WebhookId = e.WebhookId, WebhookSecret = e.WebhookSecret },
                v => new DestinationDiscordWebhook { Id = Guid.CreateVersion7(), ReminderId = id, WebhookId = v.WebhookId, WebhookSecret = v.WebhookSecret }
            );
        }
        if (item.DestinationTraqChannels is not null)
        {
            var prev = await DestinationTraqChannels.AsNoTracking()
                .Where(x => x.ReminderId == id)
                .ToArrayAsync(cancellationToken);
            DestinationTraqChannels.ApplyChanges(
                item.DestinationTraqChannels,
                prev.AsSpan(),
                e => new Domain.Models.DestinationTraqChannel { ChannelId = e.ChannelId },
                v => new DestinationTraqChannel { Id = Guid.CreateVersion7(), ReminderId = id, ChannelId = v.ChannelId }
            );
        }
        await SaveChangesAsync(cancellationToken);
        return await query
            .AsSplitQuery()
            .SelectDomainUserReminder()
            .FirstAsync(cancellationToken);
    }
}
