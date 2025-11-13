using System.Buffers;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using KnoqReminder.Domain.Exceptions;
using KnoqReminder.Domain.Models;
using KnoqReminder.Domain.Repositories;
using KnoqReminder.Infrastructure.Database.Converters;
using KnoqReminder.Utilities;
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
            RemindsWhenAbsent = (item.RemindsWhenAbsent ?? ReminderKind.None).ToString(),
            RemindsFreeEvents = (item.RemindsFreeEvents ?? ReminderKind.None).ToString(),
            AheadOfTimeReminders = [.. (item.AheadOfTimeReminderTimes ?? []).Distinct().Select(x => new AheadOfTimeReminder
            {
                Id = Guid.CreateVersion7(),
                ReminderId = reminderId,
                Duration = x.TimeSpan
            })],
            DailyReminders = [.. (item.DailyReminderTimes ?? []).Distinct().Select(x => new DailyReminder
            {
                Id = Guid.CreateVersion7(),
                ReminderId = reminderId,
                Time = x.TimeSpan
            })],
        };
        UserReminders.Add(reminder);
        await SaveChangesAsync(cancellationToken);
        return reminder.ToDomain();
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

    async ValueTask<Domain.Models.UserReminder> IUserReminderRepository.GetUserReminderAsync(Guid id, CancellationToken cancellationToken)
    {
        var reminder = await UserReminders.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(UserReminderConverter.DtoToDomainExpression)
            .FirstOrDefaultAsync(cancellationToken);
        return reminder ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, Domain.Models.UserReminder>();
    }

    async ValueTask<Domain.Models.UserReminder> IUserReminderRepository.GetUserReminderByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var reminder = await UserReminders.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(UserReminderConverter.DtoToDomainExpression)
            .FirstOrDefaultAsync(cancellationToken);
        return reminder ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, Domain.Models.UserReminder>();
    }

    async ValueTask<Domain.Models.UserReminder[]> IUserReminderRepository.GetUserRemindersByAotReminderTimeAsync(AheadOfTimeReminderTime timeFrom, AheadOfTimeReminderTime timeTo, CancellationToken cancellationToken)
    {
        Guard.IsLessThanOrEqualTo(timeFrom, timeTo);
        return await UserReminders.AsNoTracking()
            .Where(x => x.AheadOfTimeReminders.Any(y => timeFrom.TimeSpan <= y.Duration && y.Duration <= timeTo.TimeSpan))
            .Select(UserReminderConverter.DtoToDomainExpression)
            .ToArrayAsync(cancellationToken);
    }

    async ValueTask<Domain.Models.UserReminder[]> IUserReminderRepository.GetUserRemindersByDailyReminderTimeAsync(DailyReminderTime timeFrom, DailyReminderTime timeTo, CancellationToken cancellationToken)
    {
        Guard.IsLessThanOrEqualTo(timeFrom, timeTo);
        return await UserReminders.AsNoTracking()
            .Where(x => x.DailyReminders.Any(y => timeFrom.TimeSpan <= y.Time && y.Time <= timeTo.TimeSpan))
            .Select(UserReminderConverter.DtoToDomainExpression)
            .ToArrayAsync(cancellationToken);
    }

    async ValueTask<Domain.Models.UserReminder> IUserReminderRepository.UpdateUserReminderAsync(Guid id, UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken)
    {
        var entity = await UserReminders.Where(x => x.Id == id).SingleOrDefaultAsync(cancellationToken) ?? GenericThrowHelper.Throw<RepositoryKeyNotFoundException, UserReminder>();
        if (item.UserId is not null)
        {
            entity.UserId = item.UserId.Value;
        }
        if (item.RemindsWhenAbsent is not null)
        {
            entity.RemindsWhenAbsent = item.RemindsWhenAbsent.Value.ToString();
        }
        if (item.RemindsFreeEvents is not null)
        {
            entity.RemindsFreeEvents = item.RemindsFreeEvents.Value.ToString();
        }
        if (item.AheadOfTimeReminderTimes is not null)
        {
            using var current = entity.AheadOfTimeReminders.AsValueEnumerable()
                .Select(x => new AheadOfTimeReminderTime(x.Duration))
                .ToArrayPool();
            using var diff = EnumerableHelper.CompareTo<AheadOfTimeReminderTime>(item.AheadOfTimeReminderTimes, current.Span);
            foreach (var (x, d) in diff.Span)
            {
                if (d == EnumerableHelper.Difference.Add)
                {
                    AheadOfTimeReminders.Add(new()
                    {
                        Id = Guid.CreateVersion7(),
                        ReminderId = id,
                        Duration = x.TimeSpan
                    });
                }
                else if (d == EnumerableHelper.Difference.Remove)
                {
                    AheadOfTimeReminders.Remove(entity.AheadOfTimeReminders.First(y => y.Duration == x.TimeSpan));
                }
            }
        }
        if (item.DailyReminderTimes is not null)
        {
            using var current = entity.DailyReminders.AsValueEnumerable()
                .Select(x => new DailyReminderTime(TimeOnly.FromTimeSpan(x.Time)))
                .ToArrayPool();
            using var diff = EnumerableHelper.CompareTo<DailyReminderTime>(item.DailyReminderTimes, current.Span);
            foreach (var (x, d) in diff.Span)
            {
                if (d == EnumerableHelper.Difference.Add)
                {
                    DailyReminders.Add(new DailyReminder()
                    {
                        Id = Guid.CreateVersion7(),
                        ReminderId = id,
                        Time = x.TimeSpan
                    });
                }
                else if (d == EnumerableHelper.Difference.Remove)
                {
                    DailyReminders.Remove(entity.DailyReminders.First(y => y.Time == x.TimeSpan));
                }
            }
        }
        await SaveChangesAsync(cancellationToken);
        return entity.ToDomain();
    }
}

file static class EnumerableHelper
{
    public enum Difference { Zero = 0, Add = 1, Remove = -1 }

    const int MaxStackAllocationBytes = 1024;

    public static RentArray<(T, Difference)> CompareTo<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other) where T : unmanaged, IEquatable<T>
    {
        if (other.Length * 2 * Unsafe.SizeOf<T>() <= MaxStackAllocationBytes)
        {
            Span<T> bufOther = stackalloc T[other.Length];
            Span<T?> bufOtherRemains = stackalloc T?[other.Length];
            other.CopyTo(bufOther);
            return execCore(span, bufOther, bufOtherRemains);
        }
        else
        {
            using var bufOtherArray = RentArray.RentAndCreate<T>(other.Length);
            using var bufOtherRemainsArray = RentArray.RentAndCreate<T?>(other.Length);
            Span<T> bufOther = bufOtherArray.Span;
            Span<T?> bufOtherRemains = bufOtherRemainsArray.Span;
            other.CopyTo(bufOther);
            return execCore(span, bufOther, bufOtherRemains);
        }

        static RentArray<(T, Difference)> execCore(scoped ReadOnlySpan<T> @this, scoped Span<T> other, scoped Span<T?> otherRemains)
        {
            var comparer = Comparer<T>.Default;
            other.Sort(comparer);
            other.AsValueEnumerable().Select(x => new T?(x)).CopyTo(otherRemains);

            var result = RentArray.RentAndCreate<(T, Difference)>(@this.Length + other.Length);
            var resSpan = result.Span;
            for (int i = 0; i < @this.Length; i++)
            {
                var x = @this[i];
                var otherIdx = other.BinarySearch(x, comparer);
                if (otherIdx >= 0)
                {
                    resSpan[i] = (x, Difference.Zero);
                    otherRemains[otherIdx] = null;
                }
                else
                {
                    resSpan[i] = (x, Difference.Add);
                }
            }
            int remCnt = 0;
            Span<(T, Difference)> remSpan = resSpan[@this.Length..];
            foreach (var x in otherRemains.AsValueEnumerable().Where(y => y is not null))
            {
                remSpan[remCnt++] = (x!.Value, Difference.Remove);
            }
            return result.Resize(@this.Length + remCnt);
        }
    }
}
