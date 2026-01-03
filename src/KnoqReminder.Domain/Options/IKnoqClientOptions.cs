namespace KnoqReminder.Domain.Options;

public interface IKnoqClientOptions
{
    Uri ApiBaseUrl { get; }

    Uri WebPageBaseUrl { get; }

    string Username { get; }

    string Password { get; }
}
