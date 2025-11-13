using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

public class KnoqApiClientConfiguration : IKnoqApiClientOptions
{
    public const string EnvironmentPrefix = "KNOQ_";

    [ConfigurationKeyName(EnvironmentPrefix + "BASE_ADDRESS")]
    public string BaseUrl
    {
        get => _baseUrl;

        set => _baseUrl = Uri.IsWellFormedUriString(value, UriKind.Absolute) ? value : string.Empty;
    }
    string _baseUrl = string.Empty;

    [ConfigurationKeyName(EnvironmentPrefix + "USERNAME")]
    public string Username { get; set; } = string.Empty;

    [ConfigurationKeyName(EnvironmentPrefix + "PASSWORD")]
    public string Password { get; set; } = string.Empty;
}
