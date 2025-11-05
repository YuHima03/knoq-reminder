namespace KnoqReminder.Domain.Options;

public abstract class DbConnectionOptions
{
    public abstract string? ConnectionString { get; }
}
