namespace KnoqReminder.Domain.Exceptions;

public class RepositoryKeyNotFoundException : Exception
{
    public RepositoryKeyNotFoundException() { }

    public RepositoryKeyNotFoundException(string? message) : base(message) { }

    public RepositoryKeyNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}
