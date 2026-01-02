using CommunityToolkit.Diagnostics;
using KnoqReminder.Domain.Exceptions;
using KnoqReminder.Domain.Reminder.Models;
using KnoqReminder.Domain.Repositories;
using KnoqReminder.Domain.Repositories.Models;
using KnoqReminder.Domain.Services.Reminder;
using KnoqReminder.Utilities.Helpers;

namespace KnoqReminder.Tests.Domain.Reminder.Repositories;

sealed partial class RepositoryMock : IUserReminderRepository
{
    readonly Dictionary<Guid, UserReminder> _items = [];
    readonly Lock _lock = new();

    public async ValueTask<UserReminder> AddUserReminderAsync(UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(item.UserId, nameof(item.UserId));
        Guard.IsNotDefault(item.UserId.Value, nameof(item.UserId));
        var now = DateTimeOffset.UtcNow;
        var id = Guid.NewGuid();
        lock (_lock)
        {
            if (_items.Values.Any(x => x.UserId == item.UserId.Value))
            {
                ThrowHelper.ThrowInvalidOperationException("A user reminder for the specified user already exists.");
            }
            return _items[id] = new UserReminder(
                id,
                item.UserId.Value,
                item.RemindsWhenPending.GetValueOrDefault(),
                item.RemindsWhenAbsent.GetValueOrDefault(),
                item.RemindsOpenEvents.GetValueOrDefault(),
                item.AheadOfTimeReminderTimes ?? [],
                item.DailyReminderTimes ?? [],
                item.DestinationDiscordWebhooks ?? [],
                item.DestinationTraqChannels ?? [],
                now,
                now
            );
        }
    }

    public async ValueTask DeleteUserRemindersAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            foreach (var id in ids.Distinct())
            {
                _items.Remove(id, out _);
            }
        }
    }

    public void Dispose() { }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public async ValueTask<UserAotReminder[]> GetUserAotRemindersAsync(AheadOfTimeReminderTime offsetFrom, AheadOfTimeReminderTime offsetTo, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return [.._items.Values
                .SelectMany(r => r.AheadOfTimeReminderTimes
                    .Where(off => off >= offsetFrom && off <= offsetTo)
                    .Select(off => new UserAotReminder(
                        r.Id,
                        r.UserId,
                        new ReminderDestination
                        {
                            DiscordWebhooks = r.DestinationDiscordWebhooks,
                            TraqChannels = r.DestinationTraqChannels
                        },
                        off)))
                ];
        }
    }

    public async ValueTask<UserDailyReminder[]> GetUserDailyRemindersAsync(DailyReminderTime timeFrom, DailyReminderTime timeTo, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return [.. _items.Values
                .SelectMany(r => r.DailyReminderTimes
                    .Where(t => t >= timeFrom && t <= timeTo)
                    .Select(t => new UserDailyReminder(
                        r.Id,
                        r.UserId,
                        new ReminderDestination
                        {
                            DiscordWebhooks = r.DestinationDiscordWebhooks,
                            TraqChannels = r.DestinationTraqChannels
                        },
                        t)))
                ];
        }
    }

    public async ValueTask<UserReminder> GetUserReminderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return _items.TryGetValue(id, out var r) ? r : GenericThrowHelper.Throw<RepositoryKeyNotFoundException, UserReminder>();
        }
    }

    public async ValueTask<UserReminder> GetUserReminderByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return _items.Values.FirstOrDefault(x => x.UserId == userId) ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, UserReminder>();
        }
    }

    public ValueTask<UserReminderOverview> GetUserReminderOverviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<UserReminder> UpdateUserReminderAsync(Guid id, UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_items.TryGetValue(id, out var existing))
            {
                GenericThrowHelper.Throw<RepositoryKeyNotFoundException>();
            }
            return _items[id] = new UserReminder(
                existing.Id,
                item.UserId ?? existing.UserId,
                item.RemindsWhenPending ?? existing.RemindsWhenPending,
                item.RemindsWhenAbsent ?? existing.RemindsWhenAbsent,
                item.RemindsOpenEvents ?? existing.RemindsOpenEvents,
                item.AheadOfTimeReminderTimes ?? existing.AheadOfTimeReminderTimes,
                item.DailyReminderTimes ?? existing.DailyReminderTimes,
                item.DestinationDiscordWebhooks ?? existing.DestinationDiscordWebhooks,
                item.DestinationTraqChannels ?? existing.DestinationTraqChannels,
                existing.CreatedAt,
                DateTimeOffset.UtcNow);
        }
    }
}
