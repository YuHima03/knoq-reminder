using KnoqReminder.Domain.Models;
using KnoqReminder.Domain.Services.Events;

namespace KnoqReminder.App.Services.Reminder;

sealed partial class ReminderPublisher
{
    static string AotReminderTitleMarkdown => "⏰ イベント開始前のリマインドです!";

    public async ValueTask PublishAotReminderForUserAsync(
        Guid userId,
        ReminderDestination dest,
        ScheduledEvent[] events,
        CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            PublishDiscordWebhookWithEventsAsync(dest.DiscordWebhooks, "knoQ Reminder", AotReminderTitleMarkdown, events, cancellationToken),
            PublishTraqMessageWithEventAsync(dest.TraqChannels, userId, AotReminderTitleMarkdown, events, cancellationToken));
    }
}
