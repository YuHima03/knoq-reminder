using System.ComponentModel.DataAnnotations;
using KnoqReminder.Domain.Options;
using KnoqReminder.Infrastructure.Database;

namespace KnoqReminder.App.Configurations;

public class NsMariaDbConnectionConfiguration : IDbConnectionOptions
{
    public const string Prefix = "NsMariadb";

    readonly MariaDbConnectionConfiguration _config = new();

    [ConfigurationKeyName(Prefix + "Hostname")]
    public string? Host
    {
        get => _config.Host;
        set => _config.Host = value;
    }

    [ConfigurationKeyName(Prefix + "Port")]
    public int Port
    {
        get => _config.Port;
        set => _config.Port = value;
    }

    [ConfigurationKeyName(Prefix + "User")]
    public string? Username
    {
        get => _config.Username;
        set => _config.Username = value;
    }

    [ConfigurationKeyName(Prefix + "Password")]
    public string? Password
    {
        get => _config.Password;
        set => _config.Password = value;
    }

    [ConfigurationKeyName(Prefix + "Database")]
    public string? Database
    {
        get => _config.Database;
        set => _config.Database = value;
    }

    public string ConnectionString => _config.ConnectionString;

    public bool IsValid => Validator.TryValidateObject(_config, new ValidationContext(_config), null, validateAllProperties: true);
}
