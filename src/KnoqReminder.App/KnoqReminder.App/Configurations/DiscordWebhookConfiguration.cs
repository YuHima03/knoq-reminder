using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

sealed class DiscordWebhookConfiguration : IDiscordWebhookOptions
{
    public const string Position = "discord:webhooks";

    [ConfigurationKeyName("baseUrl")]
    [NotNull]
    [Required]
    public Uri? WebhookBaseUrl { get; set; }
}
