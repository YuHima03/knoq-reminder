using System.Text;
using KnoqReminder.App.Helpers.Traq;
using KnoqReminder.Domain.Repositories.Models;
using KnoqReminder.Domain.Services.DiscordWebhook;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Localization;
using KnoqReminder.Domain.Services.Reminder;
using KnoqReminder.Domain.Services.Urls;
using KnoqReminder.Utilities.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Traq;
using ZLinq;

namespace KnoqReminder.App.Services.Reminder;

public class ReminderPublisherImplement(
    IDiscordWebhookPublisher discordWebhookPublisher,
    IKnoqUrlProvider knoqUrlProvider,
    ILocalTimeProvider localTimeProvider,
    TraqApiClient traq,
    IMemoryCache cache,
    ObjectPool<Traq.Models.PostMessageRequest> postMessageRequestPool,
    ObjectPool<StringBuilder> stringBuilderPool,
    ILogger<ReminderPublisherImplement> logger,
    ILoggerFactory loggerFactory
    )
    : IReminderPublisher
{
    public ValueTask PublishAotReminderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask PublishDailyRemainderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            (dest.DiscordWebhooks.Length == 0) ? Task.CompletedTask : PublishDailyReminderForUserAsyncInternal_DiscordWebhook(dest.DiscordWebhooks, events, cancellationToken),
            (dest.TraqChannels.Length == 0) ? Task.CompletedTask : PublishDailyReminderForUserAsyncInternal_Traq(userId, dest.TraqChannels, events, cancellationToken)
        );
    }

    async Task PublishDailyReminderForUserAsyncInternal_DiscordWebhook(DestinationDiscordWebhook[] webhooks, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        using var embedsArray = await DiscordWebhookReminderHelper.GetDiscordWebhookEmbedForEventsAsync(events, cache, knoqUrlProvider, loggerFactory, traq, cancellationToken);
        using var messages = embedsArray.Span.AsValueEnumerable()
            .Chunk(DiscordWebhookReminderHelper.MaxDiscordWebhookEmbedsPerMessage)
            .Select((ems, i) => new DiscordWebhookMessage
            {
                Username = "knoQ Reminder",
                Content = (i == 0) ? $"# Today's Events: {localTimeProvider.LocalToday.ToString(ReminderConstants.TodayDateOnlyFormat)}" : null,
                Embeds = ems
            })
            .ToArrayPool();
        using var tasks = webhooks.AsValueEnumerable()
            .Select(async w =>
            {
                foreach (var msg in messages.Span)
                {
                    await discordWebhookPublisher.PublishDiscordWebhookMessageAsync(w.WebhookId, w.WebhookSecret, msg, cancellationToken).ConfigureAwait(false);
                }
            })
            .ToArrayPool();
        await Task.WhenAll(tasks.Span);
    }

    async Task PublishDailyReminderForUserAsyncInternal_Traq(Guid userId, DestinationTraqChannel[] channels, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        var user = await traq.Users[userId].TryGetCachedAsync(cache, loggerFactory, cancellationToken: cancellationToken);
        if (user is null)
        {
            return;
        }
        var sb = stringBuilderPool.Get();
        var localToday = localTimeProvider.LocalToday;
        sb.AppendLine($"# Today's Events: {localToday.ToString(ReminderConstants.TodayDateOnlyFormat)}")
            .AppendLine()
            .AppendLine($"!{{\"type\":\"user\",\"raw\":\"@{user.Name}\",\"id\":\"{userId}\"}}")
            .AppendLine()
            .AppendLine("""
                | Name | Time | Place |
                | :--- | :--- | :---- |
                """);

        foreach (var e in events)
        {
            var host = await traq.Groups[e.HostGroupId].GetAsync(cancellationToken: cancellationToken);
            // Event name and host
            sb.Append($"| **[{e.Name.Truncate(ReminderConstants.MaxEventNameLength)}]({knoqUrlProvider.GetEventPageUrl(e.Id)})**\x20");
            if (host?.Name is string hostName)
            {
                sb.Append($"by [{hostName.Truncate(ReminderConstants.MaxEventHostNameLength)}]({knoqUrlProvider.GetGroupPageUrl(e.HostGroupId)})\x20");
            }
            // Event time
            var localTodayDTOffset = localTimeProvider.ToUtcDateTime(localToday.ToDateTime(TimeOnly.MinValue));
            var startsAtLocal = localTimeProvider.ToLocalDateTime(e.StartsAt.UtcDateTime);
            sb.Append($"| {startsAtLocal.ToString((e.StartsAt < localTodayDTOffset) ? ReminderConstants.EventDateTimeFormat : ReminderConstants.EventDateTimeFormatTimeOnly)}\x20");
            var endsAtLocal = localTimeProvider.ToLocalDateTime(e.EndsAt.UtcDateTime);
            sb.Append($"~ {endsAtLocal.ToString((localTodayDTOffset.AddTicks(TimeSpan.TicksPerDay) <= e.EndsAt) ? ReminderConstants.EventDateTimeFormat : ReminderConstants.EventDateTimeFormatTimeOnly)}\x20");
            // Event place
            sb.Append($"| {e.Place.Truncate(ReminderConstants.MaxEventPlaceNameLength)} |");
            sb.AppendLine();
        }
        var postReq = postMessageRequestPool.Get();
        postReq.Content = sb.ToString();
        postReq.Embed = false;
        stringBuilderPool.Return(Interlocked.Exchange(ref sb, null));
        await Task.WhenAll(
            channels.Select(async ch => await traq.Channels[ch.ChannelId].Messages.TryPostAsync(postReq, loggerFactory, cancellationToken: cancellationToken))
        );
        postMessageRequestPool.Return(postReq);
    }
}
