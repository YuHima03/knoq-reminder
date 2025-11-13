namespace KnoqReminder.Domain.Repositories.Models;

public readonly struct DestinationDiscordWebhook
{
    public string WebhookId { get; init; }

    public string WebhookSecret { get; init; }
}
