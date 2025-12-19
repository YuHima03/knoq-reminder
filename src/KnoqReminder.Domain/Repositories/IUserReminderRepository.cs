using KnoqReminder.Domain.Repositories.Models;

namespace KnoqReminder.Domain.Repositories;

public interface IUserReminderRepository : IRepositoryBase
{
    ValueTask<UserReminder> AddUserReminderAsync(UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken = default);

    ValueTask DeleteUserRemindersAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    ValueTask<UserReminder> GetUserReminderAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<UserReminder> GetUserReminderByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    ValueTask<UserAotReminder[]> GetUserAotRemindersAsync(AheadOfTimeReminderTime offsetFrom, AheadOfTimeReminderTime offsetTo, CancellationToken cancellationToken = default);

    ValueTask<UserDailyReminder[]> GetUserDailyRemindersAsync(DailyReminderTime timeFrom, DailyReminderTime timeTo, CancellationToken cancellationToken = default);

    ValueTask<UserReminder> UpdateUserReminderAsync(Guid id, UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken = default);
}
