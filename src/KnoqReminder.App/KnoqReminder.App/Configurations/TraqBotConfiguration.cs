using KnoqReminder.Domain.Options;
using KnoqReminder.Utilities.Validation;

namespace KnoqReminder.App.Configurations;

public class TraqBotConfiguration : ITraqBotOptions
{
    public const string EnvironmentPrefix = "TRAQ_BOT_";

    [ConfigurationKeyName(EnvironmentPrefix + "USER_ID")]
    [IsNotDefaultValue<Guid>(ErrorMessage = $"The configuration {EnvironmentPrefix}USER_ID is not set or zero uuid.")]
    public Guid BotUserId { get; set; }
}
