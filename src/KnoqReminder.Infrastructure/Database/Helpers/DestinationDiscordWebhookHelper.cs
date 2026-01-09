using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DestinationDiscordWebhookHelper
{
    public static IEnumerable<Domain.Models.DestinationDiscordWebhook> SelectDomainDestinationDiscordWebhook(this IEnumerable<DestinationDiscordWebhook> dtoQueryable)
    {
        return dtoQueryable.Select(x => new Domain.Models.DestinationDiscordWebhook
        {
            WebhookId = x.WebhookId,
            WebhookSecret = x.WebhookSecret,
        });
    }
}
