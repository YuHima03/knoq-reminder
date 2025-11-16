namespace KnoqReminder.Domain.Services.DiscordWebhook;

public interface IDiscordWebhookPublisher
{
    /// <summary>
    /// Sends a message to the specified Discord webhook.
    /// </summary>
    /// <remarks>
    /// This throws no exceptions if the message could not be delivered.
    /// </remarks>
    ValueTask PublishDiscordWebhookMessageAsync(string webhookId, string webhookSecret, DiscordWebhookMessage message, CancellationToken cancellationToken = default);
}
