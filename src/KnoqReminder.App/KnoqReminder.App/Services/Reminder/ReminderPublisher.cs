using System.Text;
using KnoqReminder.App.Helpers.Traq;
using KnoqReminder.App.Services.Reminder.Helpers;
using KnoqReminder.Domain.Repositories.Models;
using KnoqReminder.Domain.Services.DiscordWebhook;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Localization;
using KnoqReminder.Domain.Services.Reminder;
using KnoqReminder.Domain.Services.Urls;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Traq;
using ZLinq;

namespace KnoqReminder.App.Services.Reminder;

sealed partial class ReminderPublisher(
    IDiscordWebhookPublisher discordWebhookPublisher,
    IKnoqUrlProvider knoqUrlProvider,
    ILocalTimeProvider localTimeProvider,
    TraqApiClient traq,
    IMemoryCache cache,
    ObjectPool<Traq.Models.PostMessageRequest> postMessageRequestPool,
    ObjectPool<StringBuilder> stringBuilderPool,
    ILogger<ReminderPublisher> logger,
    ILoggerFactory loggerFactory
    )
    : IReminderPublisher
{
    async Task PublishDiscordWebhookWithEventsAsync(
        DestinationDiscordWebhook[] webhooks,
        string authorName,
        string content,
        ScheduledEvent[] events,
        CancellationToken cancellationToken = default)
    {
        if (webhooks.Length == 0)
        {
            return;
        }
        using var eventEmbeds = await DiscordWebhookReminderHelper.GetDiscordWebhookEmbedForEventsAsync(events, cache, knoqUrlProvider, loggerFactory, traq, cancellationToken);
        var messages = DiscordWebhookReminderHelper.CreateDiscordWebhookMessages(
            username: authorName,
            content: content,
            embeds: eventEmbeds.Span);
        await Task.WhenAll([.. webhooks.AsValueEnumerable()
            .SelectMany(w => messages.AsValueEnumerable()
                .Select(msg => discordWebhookPublisher.PublishDiscordWebhookMessageAsync(w.WebhookId, w.WebhookSecret, msg, cancellationToken).AsTask()))
        ]);
    }

    async Task PublishTraqMessageWithEventAsync(
        DestinationTraqChannel[] channels,
        Guid destUserId,
        string content,
        ScheduledEvent[] events,
        CancellationToken cancellationToken = default)
    {
        if (channels.Length == 0)
        {
            return;
        }
        var user = await traq.Users[destUserId].TryGetCachedAsync(cache, loggerFactory, cancellationToken: cancellationToken);
        if (user is null)
        {
            return;
        }
        var sb = stringBuilderPool.Get();
        sb.AppendLine(content.TrimEnd())
            .AppendLine()
            .AppendTraqUserMention(user.Name, user.Id.GetValueOrDefault()).AppendLine()
            .AppendLine();
        await sb.AppendEventsTableAsync(events, cache, knoqUrlProvider, localTimeProvider, traq, cancellationToken);

        var postReq = postMessageRequestPool.Get();
        postReq.Embed = false;
        postReq.Content = sb.ToString();
        stringBuilderPool.Return(sb);
        await Task.WhenAll(
            channels.Select(ch => traq.Channels[ch.ChannelId].Messages.TryPostAsync(postReq, loggerFactory, cancellationToken: cancellationToken).AsTask())
        );
        postMessageRequestPool.Return(postReq);
    }
}
