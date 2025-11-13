namespace KnoqReminder.Domain.Repositories.Models;

public readonly struct DestinationDiscordWebhook : IComparable<DestinationDiscordWebhook>, IEquatable<DestinationDiscordWebhook>
{
    public string WebhookId { get; init; }

    public string WebhookSecret { get; init; }

    public int CompareTo(DestinationDiscordWebhook other)
    {
        return WebhookId.CompareTo(other.WebhookId);
    }

    public override bool Equals(object? obj)
    {
        return obj is DestinationDiscordWebhook other && Equals(other);
    }

    public override int GetHashCode()
    {
        return WebhookId.GetHashCode(StringComparison.Ordinal);
    }

    public bool Equals(DestinationDiscordWebhook other)
    {
        return string.Equals(WebhookId, other.WebhookId, StringComparison.Ordinal);
    }

    public static bool operator ==(DestinationDiscordWebhook left, DestinationDiscordWebhook right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DestinationDiscordWebhook left, DestinationDiscordWebhook right)
    {
        return !(left == right);
    }

    public static bool operator <(DestinationDiscordWebhook left, DestinationDiscordWebhook right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(DestinationDiscordWebhook left, DestinationDiscordWebhook right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(DestinationDiscordWebhook left, DestinationDiscordWebhook right)
    {
        return !(left <= right);
    }

    public static bool operator >=(DestinationDiscordWebhook left, DestinationDiscordWebhook right)
    {
        return !(left < right);
    }
}
