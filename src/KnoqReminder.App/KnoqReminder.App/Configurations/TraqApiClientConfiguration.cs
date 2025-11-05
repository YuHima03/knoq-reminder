using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

public class TraqApiClientConfiguration : ITraqApiClientOptions
{
    public const string EnvironmentPrefix = "TRAQ_";

    [ConfigurationKeyName(EnvironmentPrefix + "BASE_ADDRESS")]
    public string BaseUrl
    {
        get => _baseUrl;

        set => _baseUrl = Uri.IsWellFormedUriString(value, UriKind.Absolute) ? value : string.Empty;
    }
    string _baseUrl = string.Empty;

    [ConfigurationKeyName(EnvironmentPrefix + "ACCESS_TOKEN")]
    public string AccessToken { get; set; } = string.Empty;
}
