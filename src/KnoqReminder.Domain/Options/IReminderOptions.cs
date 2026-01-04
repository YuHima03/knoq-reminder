namespace KnoqReminder.Domain.Options;

public interface IReminderOptions
{
    TimeSpan FirstRemindingTaskDelay { get; }

    TimeSpan RemindingTaskTimeout { get; }
}
