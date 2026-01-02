using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

sealed class DiscordWebhookConfiguration : IDiscordWebhookOptions
{
    public const string Position = "discord:webhooks";

    [ConfigurationKeyName("baseUrl")]
    public string BaseUrlString
    {
        get => field;
        set
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out _baseUrl))
            {
                field = value;
                return;
            }
            field = string.Empty;
            _baseUrl = null;
        }
    } = string.Empty;

    Uri? IDiscordWebhookOptions.WebhookBaseUrl => _baseUrl;
    Uri? _baseUrl = null;
}
