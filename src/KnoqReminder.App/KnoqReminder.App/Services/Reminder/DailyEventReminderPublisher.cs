using KnoqReminder.Domain.Models;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Domain.Services.Reminder;

namespace KnoqReminder.App.Services.Reminder;

sealed partial class ReminderPublisher
{
    string DailyReminderTitleMarkdown => $"# 📆 {localTimeProvider.LocalToday.ToString(ReminderConstants.TodayDateOnlyFormat)} きょうのイベント";

    public async ValueTask PublishDailyReminderForUserAsync(
        Guid userId,
        ReminderDestination dest,
        ScheduledEvent[] events,
        CancellationToken cancellationToken = default)
    {
        var titleMarkdown = DailyReminderTitleMarkdown;
        await Task.WhenAll(
            PublishDiscordWebhookWithEventsAsync(dest.DiscordWebhooks, "knoQ Reminder", titleMarkdown, events, cancellationToken),
            PublishTraqMessageWithEventAsync(dest.TraqChannels, userId, titleMarkdown, events, cancellationToken));
    }
}
