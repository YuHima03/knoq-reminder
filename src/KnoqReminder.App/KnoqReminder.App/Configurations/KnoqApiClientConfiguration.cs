namespace KnoqReminder.App.Configurations;

public class KnoqApiClientConfiguration
{
    public const string EnvironmentPrefix = "KNOQ_";

    [ConfigurationKeyName(EnvironmentPrefix + "BASE_ADDRESS")]
    public string? BaseUrl { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "USERNAME")]
    public string? Username { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "PASSWORD")]
    public string? Password { get; set; }
}
