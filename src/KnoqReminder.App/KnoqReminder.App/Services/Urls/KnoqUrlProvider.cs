using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Services.Urls;
using Microsoft.Extensions.Options;

namespace KnoqReminder.App.Services.Urls;

sealed class KnoqUrlProvider(
    IOptions<IKnoqClientOptions> options
    )
    : IKnoqUrlProvider
{
    public Uri BaseUrl { get; } = options.Value.WebPageBaseUrl;

    public Uri GetEventPageUrl(Guid eventId)
    {
        return new Uri(BaseUrl, $"events/{eventId}");
    }

    public Uri GetGroupPageUrl(Guid groupId)
    {
        return new Uri(BaseUrl, $"groups/{groupId}");
    }
}
