using KnoqReminder.App.Helpers.Traq;
using KnoqReminder.Domain.Services.DiscordWebhook;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Reminder;
using KnoqReminder.Domain.Services.Urls;
using KnoqReminder.Utilities.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Traq;
using ZLinq;

namespace KnoqReminder.App.Services.Reminder.Helpers;

static class DiscordWebhookReminderHelper
{
    const string TraqGroupMapCacheKey = "Traq:GroupMap";

    public const int MaxDiscordWebhookEmbedsPerMessage = 10;

    public static DiscordWebhookMessage[] CreateDiscordWebhookMessages(
        string username,
        string content,
        ReadOnlySpan<DiscordWebhookMessage.Embed> embeds)
    {
        if (embeds.Length <= MaxDiscordWebhookEmbedsPerMessage)
        {
            return [new () {
                Username = username,
                Content = content,
                Embeds = embeds.ToArray()
            }];
        }
        return [.. embeds.AsValueEnumerable()
            .Chunk(MaxDiscordWebhookEmbedsPerMessage)
            .Select((ems, i) => new DiscordWebhookMessage
            {
                Username = username,
                Content = (i == 0) ? content : null,
                Embeds = ems
            })];
    }

    public static async ValueTask<DiscordWebhookMessage.Embed[]> GetDiscordWebhookEmbedForEventsAsync(
        ScheduledEvent[] events,
        IMemoryCache cache,
        IKnoqUrlProvider knoqUrlProvider,
        ILoggerFactory loggerFactory,
        TraqApiClient traq,
        CancellationToken cancellationToken = default)
    {
        return await events
            .ToAsyncEnumerable()
            .Select(async (e, ct) => new DiscordWebhookMessage.Embed
            {
                Author = new()
                {
                    Name = (await traq.Groups[e.HostGroupId].TryGetCachedAsync(cache, loggerFactory, cancellationToken: ct))?.Name ?? "Unknown group",
                    Url = knoqUrlProvider.GetGroupPageUrl(e.HostGroupId)
                },
                Title = e.Name.Truncate(ReminderConstants.MaxEventNameLength),
                Url = knoqUrlProvider.GetEventPageUrl(e.Id),
                Description = e.Description.Truncate(ReminderConstants.MaxEventDescriptionLength),
                Fields = [
                    new()
                    {
                        Name = "Start",
                        Value = $"<t:{e.StartsAt.ToUnixTimeSeconds()}:R>",
                    },
                    new()
                    {
                        Name = "Time",
                        Value = $"<t:{e.StartsAt.ToUnixTimeSeconds()}> ~ <t:{e.EndsAt.ToUnixTimeSeconds()}>"
                    },
                    new()
                    {
                        Name = "Place",
                        Value = e.Place.Truncate(ReminderConstants.MaxEventPlaceNameLength)
                    }
                ]
            })
            .ToArrayAsync(cancellationToken);
    }
}
