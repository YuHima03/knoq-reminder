namespace KnoqReminder.Domain.Options;

public interface IReminderOptions
{
    TimeSpan RemindingTaskTimeout { get; }
}
