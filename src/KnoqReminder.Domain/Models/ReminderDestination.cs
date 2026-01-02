namespace KnoqReminder.Domain.Models;

public class ReminderDestination
{
    public DestinationDiscordWebhook[] DiscordWebhooks { get; init; } = [];

    public DestinationTraqChannel[] TraqChannels { get; init; } = [];
}
