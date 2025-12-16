using System;
using System.Collections.Generic;
using System.Text;

namespace KnoqReminder.Domain.Options;

public interface IReminderOptions
{
    TimeSpan AotReminderTimeBeforeEvent { get; }

    TimeSpan SchedulingInterval { get; }
}
