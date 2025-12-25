namespace KnoqReminder.Domain.Options;

public interface IReminderOptions
{
    TimeSpan AotReminderTimeBeforeEvent { get; }

    TimeSpan SchedulingInterval { get; }
}
