using KnoqReminder.Domain.Options;
using KnoqReminder.Utilities.Validation;

namespace KnoqReminder.App.Configurations;

sealed class ReminderConfiguration : IReminderOptions
{
    public const string Position = "Reminder";

    [ConfigurationKeyName("FirstDelay")]
    [RangeStrict<TimeSpan>("00:00:00", null,
        ErrorMessage = "{0} must be 00:00:00 or more.")]
    public TimeSpan FirstRemindingTaskDelay { get; set; }

    [ConfigurationKeyName("TaskTimeout")]
    [RangeStrict<TimeSpan>("00:00:00", "00:05:00",
        MinimumIsExclusive = true,
        ErrorMessage = "{0} must be greater than 00:00:00, and 00:05:00 or less.")]
    public TimeSpan RemindingTaskTimeout { get; set; }
}
