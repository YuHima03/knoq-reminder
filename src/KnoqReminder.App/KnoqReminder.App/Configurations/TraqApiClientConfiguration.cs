namespace KnoqReminder.App.Configurations;

public class TraqApiClientConfiguration
{
    public const string EnvironmentPrefix = "TRAQ_";

    [ConfigurationKeyName(EnvironmentPrefix + "BASE_ADDRESS")]
    public string? BaseUrl { get; set; }
}
