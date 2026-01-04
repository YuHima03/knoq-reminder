using System.Text;
using KnoqReminder.App.Helpers.Traq;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Localization;
using KnoqReminder.Domain.Services.Reminder;
using KnoqReminder.Domain.Services.Urls;
using KnoqReminder.Utilities.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Traq;

namespace KnoqReminder.App.Services.Reminder.Helpers;

static class TraqReminderHelper
{
    public static async ValueTask<StringBuilder> AppendEventsTableAsync(
        this StringBuilder sb,
        ScheduledEvent[] events,
        IMemoryCache cache,
        IKnoqUrlProvider knoqUrlProvider,
        ILocalTimeProvider localTimeProvider,
        ILoggerFactory loggerFactory,
        TraqApiClient traq,
        CancellationToken cancellationToken = default)
    {
        sb.AppendLine("""
            | Name | Time | Place |
            | :--- | :--- | :---- |
            """);
        var localToday = localTimeProvider.LocalToday;
        foreach (var e in events)
        {
            var host = await traq.Groups[e.HostGroupId].TryGetCachedAsync(cache, loggerFactory, null, cancellationToken);
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
        return sb;
    }

    public static StringBuilder AppendTraqUserMention(
        this StringBuilder sb,
        ReadOnlySpan<char> username,
        Guid userId)
    {
        return sb.Append($"!{{\"type\":\"user\",\"raw\":\"@{username}\",\"id\":\"{userId}\"}}");
    }
}
