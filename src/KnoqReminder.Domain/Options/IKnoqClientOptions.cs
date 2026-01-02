namespace KnoqReminder.Domain.Options;

public interface IKnoqClientOptions
{
    string ApiBaseUrl { get; }

    string WebPageBaseUrl { get; }

    string Username { get; }

    string Password { get; }
}
