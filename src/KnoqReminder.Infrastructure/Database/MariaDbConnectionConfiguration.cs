using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using KnoqReminder.Utilities.Validation;
using Microsoft.Extensions.Configuration;

namespace KnoqReminder.Infrastructure.Database;

public class MariaDbConnectionConfiguration : Domain.Options.IDbConnectionOptions
{
    public const string Position = "Mariadb";

    [ConfigurationKeyName("Hostname")]
    [Required(AllowEmptyStrings = false)]
    public string? Host { get; set; }

    [ConfigurationKeyName("ExposePort")]
    [RangeStrict<int>("1", "65535")]
    public int Port { get; set; }

    [ConfigurationKeyName("User")]
    [Required(AllowEmptyStrings = false)]
    public string? Username { get; set; }

    [ConfigurationKeyName("Password")]
    [Required(AllowEmptyStrings = true)]
    public string? Password { get; set; }

    [ConfigurationKeyName("Database")]
    [Required(AllowEmptyStrings = false)]
    public string? Database { get; set; }

    public string ConnectionString => _connectionString ??= BuildConnectionString();
    string? _connectionString = null;

    string BuildConnectionString()
    {
        DefaultInterpolatedStringHandler conn = new(literalLength: 43, formattedCount: 5);
        // Host (Literal:8, Format:1)
        conn.AppendLiteral("Server=");
        conn.AppendFormatted(Host);
        conn.AppendLiteral(";");
        // Port (Literal=6, Format=1)
        conn.AppendLiteral("Port=");
        conn.AppendFormatted(Port);
        conn.AppendLiteral(";");
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
