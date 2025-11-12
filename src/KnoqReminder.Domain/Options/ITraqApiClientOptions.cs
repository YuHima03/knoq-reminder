namespace KnoqReminder.Domain.Options;

public interface ITraqApiClientOptions
{
    string BaseUrl { get; }

    string AccessToken { get; }
}
