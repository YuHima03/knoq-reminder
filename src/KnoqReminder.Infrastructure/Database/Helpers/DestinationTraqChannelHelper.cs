using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Helpers;

static class DestinationTraqChannelHelper
{
    public static IEnumerable<Domain.Models.DestinationTraqChannel> SelectDomainDestinationTraqChannel(this IEnumerable<DestinationTraqChannel> dtoQueryable)
    {
        return dtoQueryable.Select(x => new Domain.Models.DestinationTraqChannel
        {
            ChannelId = x.ChannelId,
        });
    }
}
