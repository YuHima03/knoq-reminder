using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DestinationTraqChannelHelper
{
    public static IQueryable<Domain.Models.DestinationTraqChannel> SelectDomainDestinationTraqChannel(this IQueryable<DestinationTraqChannel> dtoQueryable)
    {
        return dtoQueryable.Select(x => new Domain.Models.DestinationTraqChannel
        {
            ChannelId = x.ChannelId,
        });
    }
}
