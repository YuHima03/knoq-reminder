using KnoqReminder.Domain.Repositories.Models;

namespace KnoqReminder.Domain.Services.Reminder;

public class ReminderDestination
{
    public DestinationDiscordWebhook[] DiscordWebhooks { get; init; } = [];

    public DestinationTraqChannel[] TraqChannels { get; init; } = [];
}
