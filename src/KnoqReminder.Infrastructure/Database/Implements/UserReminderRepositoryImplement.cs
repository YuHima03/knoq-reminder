using System.Buffers;
using CommunityToolkit.Diagnostics;
using KnoqReminder.Domain.Exceptions;
using KnoqReminder.Domain.Repositories;
using KnoqReminder.Domain.Repositories.Models;
using KnoqReminder.Infrastructure.Database.Helpers;
using KnoqReminder.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using ZLinq;

namespace KnoqReminder.Infrastructure.Database;

public partial class AppDbContext : IUserReminderRepository
{
    async ValueTask<Domain.Repositories.Models.UserReminder> IUserReminderRepository.AddUserReminderAsync(UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken)
    {
        var reminderId = Guid.CreateVersion7();
        if (item.AheadOfTimeReminderTimes is { Length: > 0 } aotReminders)
        {
            AheadOfTimeReminders.AddRange(
                aotReminders.Distinct()
                    .Select(x => new AheadOfTimeReminder
                    {
                        Id = Guid.CreateVersion7(),
                        ReminderId = reminderId,
                        Offset = x.TimeSpan
                    })
            );
        }
        if (item.DailyReminderTimes is { Length: > 0 } dailyReminders)
        {
            DailyReminders.AddRange(
                dailyReminders.Distinct()
                    .Select(x => new DailyReminder
                    {
                        Id = Guid.CreateVersion7(),
                        ReminderId = reminderId,
                        Time = x.TimeSpan
                    })
            );
        }
        if (item.DestinationDiscordWebhooks is { Length: > 0 } discordWebhooks)
        {
            DestinationsDiscords.AddRange(
                discordWebhooks.DistinctBy(x => x.WebhookId)
                    .Select(x => new DestinationsDiscord
                    {
                        ReminderId = reminderId,
                        WebhookId = x.WebhookId,
                        WebhookSecret = x.WebhookSecret
                    })
            );
        }
        if (item.DestinationTraqChannels is { Length: > 0 } traqChannels)
        {
            DestinationsTraqs.AddRange(
                traqChannels.DistinctBy(x => x.ChannelId)
                    .Select(x => new DestinationsTraq
                    {
                        ReminderId = reminderId,
                        ChannelId = x.ChannelId
                    })
            );
        }
        UserReminder reminder = new()
        {
            Id = reminderId,
            UserId = item.UserId.GetValueOrDefault(),
            RemindsWhenAbsent = (item.RemindsWhenAbsent ?? ReminderKind.None).ToString(),
            RemindsFreeEvents = (item.RemindsFreeEvents ?? ReminderKind.None).ToString(),
        };
        UserReminders.Add(reminder);
        await SaveChangesAsync(cancellationToken);
        return reminder.ToDomain() with
        {
            AheadOfTimeReminderTimes = item.AheadOfTimeReminderTimes?.AsValueEnumerable().Distinct().ToArray() ?? [],
            DailyReminderTimes = item.DailyReminderTimes?.AsValueEnumerable().Distinct().ToArray() ?? [],
            DestinationDiscordWebhooks = item.DestinationDiscordWebhooks?.AsValueEnumerable().DistinctBy(x => x.WebhookId).ToArray() ?? [],
            DestinationTraqChannels = item.DestinationTraqChannels?.AsValueEnumerable().DistinctBy(x => x.ChannelId).ToArray() ?? []
        };
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

    async ValueTask<(DestinationDiscordWebhook[], DestinationTraqChannel[])> GetDestinationsAsync(Guid reminderId, CancellationToken cancellationToken = default)
    {
        var discordWebhooks = await DestinationsDiscords.AsNoTracking()
            .Where(x => x.ReminderId == reminderId)
            .Select(DestinationDiscordHelper.DtoToDomainExpression)
            .ToArrayAsync(cancellationToken);
        var traqChannels = await DestinationsTraqs.AsNoTracking()
            .Where(x => x.ReminderId == reminderId)
            .Select(DestinationTraqHelper.DtoToDomainExpression)
            .ToArrayAsync(cancellationToken);
        return (discordWebhooks, traqChannels);
    }

    async ValueTask<UserAotReminder[]> IUserReminderRepository.GetUserAotRemindersAsync(AheadOfTimeReminderTime offsetFrom, AheadOfTimeReminderTime offsetTo, CancellationToken cancellationToken)
    {
        var q = (offsetFrom == offsetTo)
            ? AheadOfTimeReminders.AsNoTracking().Where(x => x.Offset == offsetFrom.TimeSpan)
            : AheadOfTimeReminders.AsNoTracking().Where(x => offsetFrom.TimeSpan <= x.Offset && x.Offset <= offsetTo.TimeSpan);
        return await q
            .OrderBy(r => r.Offset)
            .GroupBy(r => r.ReminderId)
            .GroupJoinDestinations(
                DestinationsDiscords.AsNoTracking(),
                DestinationsTraqs.AsNoTracking())
            .SelectMany(t => t.Item1.Select(r => new UserAotReminder
            {
                ReminderId = r.ReminderId,
                Destination = t.Item2,
                Offset = new(r.Offset)
            }))
            .ToArrayAsync(cancellationToken);
    }

    async ValueTask<UserDailyReminder[]> IUserReminderRepository.GetUserDailyRemindersAsync(DailyReminderTime timeFrom, DailyReminderTime timeTo, CancellationToken cancellationTokent)
    {
        var q = (timeFrom == timeTo)
            ? DailyReminders.AsNoTracking().Where(x => x.Time == timeFrom.TimeSpan)
            : DailyReminders.AsNoTracking().Where(x => timeFrom.TimeSpan <= x.Time && x.Time <= timeTo.TimeSpan);
        return await q
            .OrderBy(r => r.Time)
            .GroupBy(r => r.ReminderId)
            .GroupJoinDestinations(
                DestinationsDiscords.AsNoTracking(),
                DestinationsTraqs.AsNoTracking())
            .SelectMany(t => t.Item1.Select(r => new UserDailyReminder
            {
                ReminderId = r.ReminderId,
                Destination = t.Item2,
                RemindsAt = new(TimeOnly.FromTimeSpan(r.Time))
            }))
            .ToArrayAsync(cancellationTokent);
    }

    async ValueTask<Domain.Repositories.Models.UserReminder> IUserReminderRepository.GetUserReminderAsync(Guid id, CancellationToken cancellationToken)
    {
        var reminder = await UserReminders.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(UserReminderHelper.DtoToDomainExpression)
            .FirstOrDefaultAsync(cancellationToken);
        var (discordWebhooks, traqChannels) = await GetDestinationsAsync(id, cancellationToken);
        return reminder ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, Domain.Repositories.Models.UserReminder>() with
        {
            DestinationDiscordWebhooks = discordWebhooks,
            DestinationTraqChannels = traqChannels
        };
    }

    async ValueTask<Domain.Repositories.Models.UserReminder> IUserReminderRepository.GetUserReminderByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var reminder = await UserReminders.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(UserReminderHelper.DtoToDomainExpression)
            .FirstOrDefaultAsync(cancellationToken);
        if (reminder is null)
        {
            return GenericThrowHelper.Throw<RepositoryKeyNotFoundException, Domain.Repositories.Models.UserReminder>();
        }
        var (discordWebhooks, traqChannels) = await GetDestinationsAsync(reminder.Id, cancellationToken);
        return reminder with
        {
            DestinationDiscordWebhooks = discordWebhooks,
            DestinationTraqChannels = traqChannels
        };
    }

    async ValueTask<Domain.Repositories.Models.UserReminder> IUserReminderRepository.UpdateUserReminderAsync(Guid id, UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken)
    {
        var entity = await UserReminders.Where(x => x.Id == id).SingleOrDefaultAsync(cancellationToken) ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, UserReminder>();
        if (item.UserId is not null)
        {
            entity.UserId = item.UserId.Value;
        }
        if (item.RemindsWhenAbsent is not null)
        {
            entity.RemindsWhenAbsent = item.RemindsWhenAbsent.Value.ToString().ToLowerInvariant();
        }
        if (item.RemindsFreeEvents is not null)
        {
            entity.RemindsFreeEvents = item.RemindsFreeEvents.Value.ToString().ToLowerInvariant();
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
            var prev = await DestinationsDiscords.AsNoTracking()
                .Where(x => x.ReminderId == id)
                .ToArrayAsync(cancellationToken);
            DestinationsDiscords.ApplyChangesSlow(
                item.DestinationDiscordWebhooks,
                prev.AsSpan(),
                e => new DestinationDiscordWebhook { WebhookId = e.WebhookId, WebhookSecret = e.WebhookSecret },
                v => new DestinationsDiscord { ReminderId = id, WebhookId = v.WebhookId, WebhookSecret = v.WebhookSecret }
            );
        }
        if (item.DestinationTraqChannels is not null)
        {
            var prev = await DestinationsTraqs.AsNoTracking()
                .Where(x => x.ReminderId == id)
                .ToArrayAsync(cancellationToken);
            DestinationsTraqs.ApplyChanges(
                item.DestinationTraqChannels,
                prev.AsSpan(),
                e => new DestinationTraqChannel { ChannelId = e.ChannelId },
                v => new DestinationsTraq { ReminderId = id, ChannelId = v.ChannelId }
            );
        }
        await SaveChangesAsync(cancellationToken);
        return entity.ToDomain();
    }
}
