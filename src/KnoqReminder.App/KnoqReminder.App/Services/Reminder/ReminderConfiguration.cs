using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Services.Reminder;

sealed class ReminderConfiguration : IReminderOptions
{
    public const string Position = "Reminder";

    [ConfigurationKeyName("TaskTimeout")]
    public TimeSpan RemindingTaskTimeout { get; set; }
}
