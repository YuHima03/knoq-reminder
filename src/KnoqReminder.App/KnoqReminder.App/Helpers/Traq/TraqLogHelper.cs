using System.Net;
using Microsoft.Kiota.Abstractions;

namespace KnoqReminder.App.Helpers.Traq;

static class TraqLogHelper
{
    static ILogger CreateLogger(ILoggerFactory factory)
    {
        return factory.CreateLogger(typeof(TraqLogHelper));
    }

    public static async ValueTask<global::Traq.Models.UserDetail?> GetWithLogOnFailureAsync(
        this global::Traq.Users.Item.WithUserItemRequestBuilder builder,
        ILoggerFactory loggerFactory,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default
        )
    {
        try
        {
            return await builder.GetAsync(requestConfiguration, cancellationToken);
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
        {
            var logger = CreateLogger(loggerFactory);
            if (logger.IsEnabled(LogLevel.Error))
            {
                var userId = builder.ToGetRequestInformation(requestConfiguration).PathParameters["userId"];
                if (userId is Guid uid)
                {
                    logger.LogError_FailedToGetUser_UserNotFound(uid);
                }
                else
                {
                    logger.LogError_FailedToGetUser(ex);
                }
            }
        }
        catch (ApiException ex)
        {
            var logger = CreateLogger(loggerFactory);
            logger.LogError_FailedToGetUser(ex);
        }
        return null;
    }

    public static async ValueTask<global::Traq.Models.Message?> PostWithLogOnFailureAsync(
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
    /// User not found: {<paramref name="userId"/>}
    /// </summary>
    [LoggerMessage(Level = LogLevel.Error, Message = "User not found: {userId}")]
    public static partial void LogError_FailedToGetUser_UserNotFound(this ILogger logger, Guid userId);

    /// <summary>
    /// Failed to get a traQ user.
    /// </summary>
    [LoggerMessage(Level =LogLevel.Error,Message ="Failed to get a traQ user.")]
    public static partial void LogError_FailedToGetUser(this ILogger logger, Exception exception);

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
