using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

sealed class ReminderConfiguration : IReminderOptions
{
    public const string Position = "Reminder";

    [ConfigurationKeyName("TaskTimeout")]
    public TimeSpan RemindingTaskTimeout { get; set; }
}
