using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

public class KnoqClientConfiguration : IKnoqClientOptions
{
    public const string EnvironmentPrefix = "KNOQ_";

    [ConfigurationKeyName(EnvironmentPrefix + "API_BASE_ADDRESS")]
    public string ApiBaseUrl
    {
        get;
        set => field = Uri.IsWellFormedUriString(value, UriKind.Absolute) ? value : string.Empty;
    } = string.Empty;

    [ConfigurationKeyName(EnvironmentPrefix + "WEB_PAGE_BASE_ADDRESS")]
    public string WebPageBaseUrl
    {
        get;
        set => field = Uri.IsWellFormedUriString(value, UriKind.Absolute) ? value : string.Empty;
    } = string.Empty;

    [ConfigurationKeyName(EnvironmentPrefix + "USERNAME")]
    public string Username { get; set; } = string.Empty;

    [ConfigurationKeyName(EnvironmentPrefix + "PASSWORD")]
    public string Password { get; set; } = string.Empty;
}
