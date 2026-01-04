namespace KnoqReminder.Domain.Options;

public interface ITraqClientOptions
{
    Uri ApiBaseUrl { get; }

    string AccessToken { get; }
}
