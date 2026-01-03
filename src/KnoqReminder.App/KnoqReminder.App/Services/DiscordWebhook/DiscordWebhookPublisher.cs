using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Services.DiscordWebhook;
using Microsoft.Extensions.Options;

namespace KnoqReminder.App.Services.DiscordWebhook;

sealed class DiscordWebhookPublisher(
    IOptions<IDiscordWebhookOptions> options,
    IHttpClientFactory httpClientFactory,
    ILogger<DiscordWebhookPublisher> logger
    )
    : IDiscordWebhookPublisher
{
    static readonly MediaTypeHeaderValue JsonMediaTypeHeader = new("application/json");

    public async ValueTask PublishDiscordWebhookMessageAsync(string webhookId, string webhookSecret, DiscordWebhookMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            using var content = JsonContent.Create(message, DiscordWebhookJsonSerializerContext.Default.DiscordWebhookMessage, JsonMediaTypeHeader);
            using var client = httpClientFactory.CreateClient();
            using var response = await client.PostAsync(new Uri(options.Value.WebhookBaseUrl!, $"{webhookId}/{webhookSecret}"), content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError_DiscordWebhookResponse(response.StatusCode, response.Content);
            }
        }
        catch (Exception ex)
        {
            logger.LogError_FailedToSendDiscordWebhook(ex);
        }
    }
}

static partial class MessageLogger
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Discord webhook returned {StatusCode} -> {Response}")]
    public static partial void LogError_DiscordWebhookResponse(this ILogger logger, System.Net.HttpStatusCode statusCode, HttpContent response);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to send discord webhook")]
    public static partial void LogError_FailedToSendDiscordWebhook(this ILogger logger, Exception ex);
}

[JsonSerializable(typeof(DiscordWebhookMessage))]
sealed partial class DiscordWebhookJsonSerializerContext : JsonSerializerContext;
