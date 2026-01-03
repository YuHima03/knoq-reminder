using System.ComponentModel.DataAnnotations;
using KnoqReminder.Domain.Options;
using KnoqReminder.Utilities.Validation;

namespace KnoqReminder.App.Configurations;

sealed class ReminderConfiguration : IReminderOptions
{
    public const string Position = "Reminder";

    [ConfigurationKeyName("TaskTimeout")]
    [Required]
    [RangeStrict<TimeSpan>("00:00:00", "00:05:00",
        MinimumIsExclusive = true,
        ErrorMessage = "`TaskTimeout` must be greater than 0, and 5min or less.")]
    public TimeSpan RemindingTaskTimeout { get; set; }
}
