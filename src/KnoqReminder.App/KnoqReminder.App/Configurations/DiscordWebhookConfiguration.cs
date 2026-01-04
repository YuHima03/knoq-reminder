using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

sealed class DiscordWebhookConfiguration : IDiscordWebhookOptions
{
    public const string Position = "Discord:Webhooks";

    [ConfigurationKeyName("BaseUrl")]
    [NotNull]
    [Required]
    public Uri? WebhookBaseUrl { get; set; }
}
