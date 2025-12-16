using System.Collections.Frozen;
using KnoqReminder.App.Helpers.Traq;
using KnoqReminder.Domain.Services.DiscordWebhook;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Reminder;
using KnoqReminder.Domain.Services.Urls;
using KnoqReminder.Utilities.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Traq;
using ZLinq;

namespace KnoqReminder.App.Services.Reminder;

static class DiscordWebhookReminderHelper
{
    const string TraqGroupMapCacheKey = "Traq:GroupMap";

    public const int MaxDiscordWebhookEmbedsPerMessage = 10;

    public static async ValueTask<PooledArray<DiscordWebhookMessage.Embed>> GetDiscordWebhookEmbedForEventsAsync(
        ScheduledEvent[] events,
        IMemoryCache cache,
        IKnoqUrlProvider knoqUrlProvider,
        ILoggerFactory loggerFactory,
        TraqApiClient traq,
        CancellationToken cancellationToken = default)
    {
        var groupNames = await cache.GetOrCreateAsync(TraqGroupMapCacheKey, async entry =>
        {
            var list = await traq.Groups.TryGetAsync(loggerFactory, cancellationToken: cancellationToken);
            entry.SetAbsoluteExpiration(DateTimeOffset.UtcNow + TimeSpan.FromMinutes(3));
            return list?.ToFrozenDictionary(g => g.Id.GetValueOrDefault(), g => g.Name);
        });
        return events.AsValueEnumerable()
            .Select(e => new DiscordWebhookMessage.Embed
            {
                Author = new()
                {
                    Name = groupNames?.GetValueOrDefault(e.HostGroupId) ?? "Unknown group",
                    Url = knoqUrlProvider.GetGroupPageUrl(e.HostGroupId)
                },
                Title = e.Name.Truncate(ReminderConstants.MaxEventNameLength),
                Url = knoqUrlProvider.GetEventPageUrl(e.Id),
                Description = e.Description.Truncate(ReminderConstants.MaxEventDescriptionLength),
                Fields = [
                    new()
                    {
                        Name = "Time",
                        Value = $"{e.StartsAt.ToString(ReminderConstants.EventDateTimeFormat)} ~ {e.EndsAt.ToString(ReminderConstants.EventDateTimeFormat)}"
                    },
                    new()
                    {
                        Name = "Place",
                        Value = e.Place.Truncate(ReminderConstants.MaxEventPlaceNameLength)
                    }
                ]
            })
            .ToArrayPool();
    }
}
