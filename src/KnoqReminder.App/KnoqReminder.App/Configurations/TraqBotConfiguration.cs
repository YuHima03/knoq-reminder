using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

public class TraqBotConfiguration : ITraqBotOptions
{
    public const string EnvironmentPrefix = "TRAQ_BOT_";

    [ConfigurationKeyName(EnvironmentPrefix + "ID")]
    public string Id { get; set; } = string.Empty;

    Guid ITraqBotOptions.Id
    {
        get
        {
            if (_id == Guid.Empty && Guid.TryParse(Id, out var guid))
            {
                return _id = guid;
            }
            return _id;
        }
    }
    Guid _id = Guid.Empty;
}
