using KnoqReminder.Domain.Options;
using KnoqReminder.Utilities.Validation;

namespace KnoqReminder.App.Configurations;

public class TraqBotConfiguration : ITraqBotOptions
{
    public const string Position = "Traq:Bot";

    [ConfigurationKeyName("UserId")]
    [IsNotDefaultValue<Guid>(ErrorMessage = $"The configuration {Position}:UserId is not set or zero uuid.")]
    public Guid BotUserId { get; set; }
}
