namespace KnoqReminder.Domain.Options;

public interface IReminderOptions
{
    TimeSpan AheadOfTimeReminderOffset { get; }

    TimeSpan SchedulingInterval { get; }
}
