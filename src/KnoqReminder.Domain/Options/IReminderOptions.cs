namespace KnoqReminder.Domain.Options;

public interface IReminderOptions
{
    TimeSpan SchedulingInterval { get; }
}
