using KnoqReminder.Domain.Models;

namespace KnoqReminder.Domain.Repositories;

/// <summary>
/// Defines methods for managing <see cref="UserReminder"/> entities in the repository.
/// </summary>
public interface IUserReminderRepository : IRepositoryBase
{
    /// <summary>
    /// Adds a new user reminder.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// <see cref="UserReminderAddOrUpdateRequest.UserId"/> is null or zero.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The specified user ID is already associated with an existing user reminder.
    /// </exception>
    ValueTask<UserReminder> AddUserReminderAsync(UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes user reminders by their IDs.
    /// </summary>
    /// <remarks>
    /// This method throws no exception even if some of the specified IDs do not exist.
    /// </remarks>
    ValueTask DeleteUserRemindersAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user reminder by its ID.
    /// </summary>
    /// <exception cref="Exceptions.RepositoryKeyNotFoundException">
    /// A user reminder with the specified ID does not exist.
    /// </exception>
    ValueTask<UserReminder> GetUserReminderAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an overview of user reminder by its ID.
    /// </summary>
    /// <exception cref="Exceptions.RepositoryKeyNotFoundException">
    /// A user reminder with the specified ID does not exist.
    /// </exception>
    ValueTask<UserReminderOverview> GetUserReminderOverviewAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user reminder by the user ID.
    /// </summary>
    /// <exception cref="Exceptions.RepositoryKeyNotFoundException">
    /// A user reminder with the specified user ID does not exist.
    /// </exception>
    ValueTask<UserReminder> GetUserReminderByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves user ahead-of-time reminders within the specified offset range.
    /// </summary>
    /// <remarks>
    /// This method throws no exception even if the offset range is invalid (i.e., <paramref name="offsetFrom"/> &gt; <paramref name="offsetTo"/>).
    /// </remarks>
    ValueTask<UserAotReminder[]> GetUserAotRemindersAsync(AheadOfTimeReminderTime offsetFrom, AheadOfTimeReminderTime offsetTo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves user daily reminders within the specified time range.
    /// </summary>
    /// <remarks>
    /// This method throws no exception even if the offset range is invalid (i.e., <paramref name="timeFrom"/> &gt; <paramref name="timeTo"/>).
    /// </remarks>
    ValueTask<UserDailyReminder[]> GetUserDailyRemindersAsync(DailyReminderTime timeFrom, DailyReminderTime timeTo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user reminder by its ID.
    /// </summary>
    /// <exception cref="Exceptions.RepositoryKeyNotFoundException">
    /// A user reminder with the specified ID does not exist.
    /// </exception>
    ValueTask<UserReminder> UpdateUserReminderAsync(Guid id, UserReminderAddOrUpdateRequest item, CancellationToken cancellationToken = default);
}
