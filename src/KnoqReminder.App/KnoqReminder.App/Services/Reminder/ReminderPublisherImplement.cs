using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using KnoqReminder.Domain.Repositories.Models;
using KnoqReminder.Domain.Services.DiscordWebhook;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Localization;
using KnoqReminder.Domain.Services.Reminder;
using KnoqReminder.Domain.Services.Urls;
using KnoqReminder.Utilities.Helpers;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Kiota.Abstractions;
using Traq;
using ZLinq;

namespace KnoqReminder.App.Services.Reminder;

public class ReminderPublisherImplement(
    IDiscordWebhookPublisher discordWebhookPublisher,
    IKnoqUrlProvider knoqUrlProvider,
    ILocalTimeProvider localTimeProvider,
    TraqApiClient traq,
    ObjectPool<Traq.Models.PostMessageRequest> postMessageRequestPool,
    ILogger<ReminderPublisherImplement> logger
    )
    : IReminderPublisher
{
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    const string EventDateTimeFormat = "MM/dd(ddd) HH:mm";

    const int MaxDiscordWebhookEmbedsPerMessage = 10;

    public ValueTask PublishAotReminderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask PublishDailyRemainderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        string titleTextMd = $"# Today's Events: {localTimeProvider.LocalToday:MM/dd (ddd)}";
        using var publishTasks = ValueEnumerableExtensions.Concat(
            publishDiscordWebhook(dest.DiscordWebhooks, events, titleTextMd, discordWebhookPublisher, knoqUrlProvider, localTimeProvider, cancellationToken),
            publishTraq()
        ).ToArrayPool();
        await Task.WhenAll(publishTasks.Span);

        static ValueEnumerable<ZLinq.Linq.FromEnumerable<Task>, Task> publishDiscordWebhook(
            DestinationDiscordWebhook[] dest,
            ScheduledEvent[] events,
            string titleTextMd,
            IDiscordWebhookPublisher discordWebhookPublisher,
            IKnoqUrlProvider knoqUrlProvider,
            ILocalTimeProvider localTimeProvider,
            CancellationToken cancellationToken)
        {
            IEnumerable<Task> tasks = [];
            if (dest.Length == 0)
            {
                return tasks.AsValueEnumerable();
            }
            using var embedsArray = events.AsValueEnumerable()
                .Select(e => new DiscordWebhookMessage.Embed
                {
                    Author = new()
                    {
                        Name = "{Host name (ToDo)}",
                        Url = knoqUrlProvider.GetGroupPageUrl(e.HostGroupId)
                    },
                    Title = e.Name.Truncate(256),
                    Url = knoqUrlProvider.GetEventPageUrl(e.Id),
                    Description = e.Description.Truncate(2048),
                    Fields = [
                        new()
                        {
                            Name = "Time",
                            Value = $"{e.StartsAt.ToString(EventDateTimeFormat)} ~ {e.EndsAt.ToString(EventDateTimeFormat)}"
                        },
                        new()
                        {
                            Name = "Place",
                            Value = e.Place.Truncate(1024)
                        }
                    ]
                })
                .ToArrayPool();
            var embeds = embedsArray.Span;
            for (int i = 0; i < embeds.Length; i += MaxDiscordWebhookEmbedsPerMessage)
            {
                DiscordWebhookMessage msg = new()
                {
                    Username = "knoQ Reminder",
                    Content = (i == 0) ? titleTextMd : null,
                    Embeds = [.. embeds.Slice(i, Math.Min(MaxDiscordWebhookEmbedsPerMessage, embeds.Length - i))]
                };
                tasks = tasks.Concat(
                    dest.Select(w => discordWebhookPublisher.PublishDiscordWebhookMessageAsync(w.WebhookId, w.WebhookSecret, msg, cancellationToken).AsTask())
                );
            }
            return tasks.AsValueEnumerable();
        }

        static ValueEnumerable<ZLinq.Linq.FromEnumerable<Task>, Task> publishTraq()
        {
            return default;
        }
    }

    async ValueTask SendTraqMessageAsync(Guid channelId, string message, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNullOrEmpty(message);
        var req = postMessageRequestPool.Get();
        try
        {
            req.Content = message;
            req.Embed = false;
            await traq.Channels[channelId].Messages.PostAsync(req, cancellationToken: cancellationToken);
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == StatusCodes.Status404NotFound)
        {
            logger.LogError_FailedToSendTraqMessage_ChannelNotFound(channelId);
        }
        catch (Exception ex)
        {
            logger.LogError_FailedToSendTraqMessage(ex);
        }
        finally
        {
            req.Content = null;
            postMessageRequestPool.Return(req);
        }
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
