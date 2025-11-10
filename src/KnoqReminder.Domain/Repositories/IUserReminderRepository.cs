using KnoqReminder.Domain.Models;

namespace KnoqReminder.Domain.Repositories;

public interface IUserReminderRepository : IRepositoryBase
{
    ValueTask<UserReminder> AddUserReminderAsync(UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken = default);

    ValueTask<UserReminder> DeleteUserRemindersAsync(ReadOnlySpan<Guid> ids, CancellationToken cancellationToken = default);

    ValueTask<UserReminder> GetUserReminderAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<UserReminder> GetUserReminderByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    ValueTask<UserReminder[]> GetUserRemindersByAotReminderTimeAsync(TimeSpan timeSpanFrom, TimeSpan timeSpan, CancellationToken cancellationToken = default);

    ValueTask<UserReminder[]> GetUserRemindersByDailyReminderTimeAsync(TimeOnly timeFrom, TimeSpan timeSpan, CancellationToken cancellationToken = default);

    ValueTask<UserReminder> UpdateUserReminderAsync(Guid id, UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken = default);
}
