namespace KnoqReminder.Domain.Options;

public interface IKnoqApiClientOptions
{
    string BaseUrl { get; }

    string Username { get; }

    string Password { get; }
}
