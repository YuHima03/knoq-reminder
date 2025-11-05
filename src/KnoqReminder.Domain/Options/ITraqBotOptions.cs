namespace KnoqReminder.Domain.Options;

public interface ITraqBotOptions
{
    Guid Id { get; }

    string AccessToken { get; }
}
