namespace KnoqReminder.Domain.Services.Urls;

public interface IKnoqUrlProvider
{
    Uri BaseUrl { get; }

    Uri GetEventPageUrl(Guid eventId);

    Uri GetGroupPageUrl(Guid groupId);
}
