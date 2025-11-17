using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommunityToolkit.Diagnostics;
using KnoqReminder.App.Helpers.Traq;
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
    ObjectPool<StringBuilder> stringBuilderPool,
    ILogger<ReminderPublisherImplement> logger,
    ILoggerFactory loggerFactory
    )
    : IReminderPublisher
{
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    const string EventDateTimeFormat = "MM/dd(ddd) HH:mm";

    [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)]
    const string TodayDateOnlyFormat = "MM/dd (ddd)";

    const int MaxDiscordWebhookEmbedsPerMessage = 10;
    const int MaxEventNameLength = 256;
    const int MaxEventHostNameLength = 256;
    const int MaxEventPlaceNameLength = 1024;
    const int MaxEventDescriptionLength = 2048;

    public ValueTask PublishAotReminderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask PublishDailyRemainderForUserAsync(Guid userId, ReminderDestination dest, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            (dest.DiscordWebhooks.Length == 0) ? Task.CompletedTask : PublishDailyReminderForUserAsyncInternal_DiscordWebhook(userId, dest.DiscordWebhooks, events, cancellationToken),
            (dest.TraqChannels.Length == 0) ? Task.CompletedTask : PublishDailyReminderForUserAsyncInternal_Traq(userId, dest.TraqChannels, events, cancellationToken)
        );
    }

    async Task PublishDailyReminderForUserAsyncInternal_DiscordWebhook(Guid userId, DestinationDiscordWebhook[] webhooks, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        using var tasks = events.AsValueEnumerable()
            .Select(e => new DiscordWebhookMessage.Embed
            {
                Author = new()
                {
                    Name = "{Host name (ToDo)}",
                    Url = knoqUrlProvider.GetGroupPageUrl(e.HostGroupId)
                },
                Title = e.Name.Truncate(MaxEventNameLength),
                Url = knoqUrlProvider.GetEventPageUrl(e.Id),
                Description = e.Description.Truncate(MaxEventDescriptionLength),
                Fields = [
                    new()
                    {
                        Name = "Time",
                        Value = $"{e.StartsAt.ToString(EventDateTimeFormat)} ~ {e.EndsAt.ToString(EventDateTimeFormat)}"
                    },
                    new()
                    {
                        Name = "Place",
                        Value = e.Place.Truncate(MaxEventPlaceNameLength)
                    }
                ]
            })
            .Chunk(MaxDiscordWebhookEmbedsPerMessage)
            .SelectMany((ems, i) =>
            {
                DiscordWebhookMessage msg = new()
                {
                    Username = "knoQ Reminder",
                    Content = (i == 0) ? $"# Today's Events: {localTimeProvider.LocalToday.ToString(TodayDateOnlyFormat)}" : null,
                    Embeds = ems
                };
                return webhooks.AsValueEnumerable().Select(w => discordWebhookPublisher.PublishDiscordWebhookMessageAsync(w.WebhookId, w.WebhookSecret, msg, cancellationToken).AsTask());
            })
            .ToArrayPool();
        await Task.WhenAll(tasks.Span);
    }

    async Task PublishDailyReminderForUserAsyncInternal_Traq(Guid userId, DestinationTraqChannel[] channels, ScheduledEvent[] events, CancellationToken cancellationToken = default)
    {
        var user = await traq.Users[userId].GetAsync(cancellationToken: cancellationToken);
        if (user is null)
        {
            return;
        }
        var sb = stringBuilderPool.Get();
        sb.AppendLine($"# Today's Events: {localTimeProvider.LocalToday.ToString(TodayDateOnlyFormat)}")
            .AppendLine()
            .AppendLine($"!{{\"type\":\"user\",\"raw\":\"@{user.Name}\",\"id\":\"{userId}\"}}")
            .AppendLine();
        foreach (var e in events)
        {
            var host = await traq.Groups[e.HostGroupId].GetAsync(cancellationToken: cancellationToken);
            if (host is null)
            {
                continue;
            }
            sb.AppendLine($"""
                    ## [{e.Name.Truncate(MaxEventNameLength)}]({knoqUrlProvider.GetEventPageUrl(e.Id)})

                    - Host: [{host.Name?.Truncate(MaxEventHostNameLength)}]({knoqUrlProvider.GetGroupPageUrl(e.HostGroupId)})
                    - Time: {e.StartsAt.ToString(EventDateTimeFormat)} ~ {e.EndsAt.ToString(EventDateTimeFormat)}
                    - Place: {e.Place.Truncate(MaxEventPlaceNameLength)}

                    """);
        }
        var postReq = postMessageRequestPool.Get();
        postReq.Content = sb.ToString();
        postReq.Embed = false;
        stringBuilderPool.Return(Interlocked.Exchange(ref sb, null));
        await Task.WhenAll(
            channels.Select(async ch => await traq.Channels[ch.ChannelId].Messages.PostWithLogOnFailureAsync(postReq, loggerFactory, cancellationToken: cancellationToken))
        );
        postMessageRequestPool.Return(postReq);
    }
}
