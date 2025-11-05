using System.Runtime.CompilerServices;

namespace KnoqReminder.App.Configurations;

public class MariaDbConnectionConfiguration : Domain.Options.IDbConnectionOptions
{
    public const string EnvironmentPrefix = "MARIADB_";

    [ConfigurationKeyName(EnvironmentPrefix + "HOST")]
    public string? Host { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "PORT")]
    public string? Port { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "USER")]
    public string? Username { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "PASSWORD")]
    public string? Password { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "DATABASE")]
    public string? Database { get; set; }
    public string? ConnectionString => _connectionString ??= BuildConnectionString();
    string? _connectionString = null;

    string BuildConnectionString()
    {
        DefaultInterpolatedStringHandler conn = new(literalLength: 43, formattedCount: 5);
        // Host (Literal:8, Format:1)
        conn.AppendLiteral("Server=");
        conn.AppendFormatted(Host);
        conn.AppendLiteral(";");
        // Port (Literal=6, Format=1)
        if (int.TryParse(Port, out var p))
        {
            conn.AppendLiteral("Port=");
            conn.AppendFormatted(p);
            conn.AppendLiteral(";");
        }
        // Username (Literal=9, Format=1)
        conn.AppendLiteral("User Id=");
        conn.AppendFormatted(Username);
        conn.AppendLiteral(";");
        // Password (Literal=10, Format=1)
        if (!string.IsNullOrEmpty(Password))
        {
            conn.AppendLiteral("Password=");
            conn.AppendFormatted(Password);
            conn.AppendLiteral(";");
        }
        // Database (Literal=10, Format=1)
        if (!string.IsNullOrEmpty(Database))
        {
            conn.AppendLiteral("Database=");
            conn.AppendFormatted(Database);
            conn.AppendLiteral(";");
        }
        return conn.ToStringAndClear();
    }
}
