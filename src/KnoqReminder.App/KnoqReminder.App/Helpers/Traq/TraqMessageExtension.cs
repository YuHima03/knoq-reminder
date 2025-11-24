using System.Net;
using Microsoft.Kiota.Abstractions;

namespace KnoqReminder.App.Helpers.Traq;

static class TraqMessageExtension
{
    static ILogger CreateLogger(ILoggerFactory loggerFactory)
    {
        return loggerFactory.CreateLogger(typeof(TraqMessageExtension));
    }

    public static async ValueTask<global::Traq.Models.Message?> TryPostAsync(
        this global::Traq.Channels.Item.Messages.MessagesRequestBuilder builder,
        global::Traq.Models.PostMessageRequest body,
        ILoggerFactory loggerFactory,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await builder.PostAsync(body, requestConfiguration, cancellationToken);
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
        {
            var logger = CreateLogger(loggerFactory);
            if (logger.IsEnabled(LogLevel.Error))
            {
                var channelId = builder.ToPostRequestInformation(body, requestConfiguration).PathParameters["channelId"];
                if (channelId is Guid cid)
                {
                    logger.LogError_FailedToSendTraqMessage_ChannelNotFound(cid);
                }
                else
                {
                    logger.LogError_FailedToSendTraqMessage(ex);
                }
            }
        }
        catch (ApiException ex)
        {
            var logger = CreateLogger(loggerFactory);
            logger.LogError_FailedToSendTraqMessage(ex);
        }
        return null;
    }
}

static partial class MessageLogger
{
    /// <summary>
    /// Failed to send a message to traQ: channel not found: {<paramref name="channelId"/>}
    /// </summary>
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to send a message to traQ: channel not found: {ChannelId}")]
    public static partial void LogError_FailedToSendTraqMessage_ChannelNotFound(this ILogger logger, Guid channelId);

    /// <summary>
    /// Failed to send a message to traQ.
    /// </summary>
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to send a message to traQ.")]
    public static partial void LogError_FailedToSendTraqMessage(this ILogger logger, Exception exception);
}
