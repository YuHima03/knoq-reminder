namespace KnoqReminder.Domain.Models;

public readonly struct DestinationTraqChannel : IComparable<DestinationTraqChannel>, IEquatable<DestinationTraqChannel>
{
    public Guid ChannelId { get; init; }

    public int CompareTo(DestinationTraqChannel other)
    {
        return ChannelId.CompareTo(other.ChannelId);
    }

    public bool Equals(DestinationTraqChannel other)
    {
        return ChannelId.Equals(other.ChannelId);
    }
    public override bool Equals(object? obj)
    {
        return obj is DestinationTraqChannel other && Equals(other);
    }

    public override int GetHashCode()
    {
        return ChannelId.GetHashCode();
    }

    public static bool operator ==(DestinationTraqChannel left, DestinationTraqChannel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DestinationTraqChannel left, DestinationTraqChannel right)
    {
        return !(left == right);
    }

    public static bool operator <(DestinationTraqChannel left, DestinationTraqChannel right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(DestinationTraqChannel left, DestinationTraqChannel right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(DestinationTraqChannel left, DestinationTraqChannel right)
    {
        return !(left <= right);
    }

    public static bool operator >=(DestinationTraqChannel left, DestinationTraqChannel right)
    {
        return !(left < right);
    }
}
