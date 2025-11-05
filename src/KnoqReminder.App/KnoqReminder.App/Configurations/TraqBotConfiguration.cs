namespace KnoqReminder.App.Configurations;

public class TraqBotConfiguration
{
    public const string EnvironmentPrefix = "TRAQ_BOT_";

    [ConfigurationKeyName(EnvironmentPrefix + "ID")]
    public string? Id { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "ACCESS_TOKEN")]
    public string? AccessToken { get; set; }
}
