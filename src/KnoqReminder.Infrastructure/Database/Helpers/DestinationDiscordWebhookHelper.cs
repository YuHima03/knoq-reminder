using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DestinationDiscordWebhookHelper
{
    public static IQueryable<Domain.Models.DestinationDiscordWebhook> SelectDomainDestinationDiscordWebhook(this IQueryable<DestinationDiscordWebhook> dtoQueryable)
    {
        return dtoQueryable.Select(x => new Domain.Models.DestinationDiscordWebhook
        {
            WebhookId = x.WebhookId,
            WebhookSecret = x.WebhookSecret,
        });
    }
}
