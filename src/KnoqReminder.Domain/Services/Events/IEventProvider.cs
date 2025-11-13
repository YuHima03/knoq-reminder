using System;
using System.Collections.Generic;
using System.Text;

namespace KnoqReminder.Domain.Services.Events;

public interface IEventProvider
{
    ValueTask<ScheduledEvent?> GetScheduleAsync(Guid id, CancellationToken cancellationToken = default);
}
