using System;
using System.Collections.Generic;
using System.Text;

namespace KnoqReminder.Domain.Services.Events;

public record ScheduledEvent(
    Guid Id,
    string Name,
    string Place,
    string Description,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt
    );
