using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnoqReminder.Domain.Models;

namespace KnoqReminder.Domain.Repositories;

public interface IUserReminderRepository : IRepositoryBase
{
    ValueTask<UserReminder> GetUserReminderAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<UserReminder> GetUserReminderByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    ValueTask<UserReminder[]> GetUserRemindersByAotReminderTimeAsync(TimeSpan timeSpanFrom, TimeSpan timeSpan, CancellationToken cancellationToken = default);

    ValueTask<UserReminder[]> GetUserRemindersByDailyReminderTimeAsync(TimeOnly timeFrom, TimeSpan timeSpan, CancellationToken cancellationToken = default);
}
