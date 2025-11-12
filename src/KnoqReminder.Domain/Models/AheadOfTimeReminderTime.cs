using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace KnoqReminder.Domain.Models;

/// <summary>
/// Represents a specific time offset before a scheduled event when a reminder should be triggered, within the range 00:00 (reminding on time) to 24:00 (reminding a day before).
/// Seconds and smaller units are not supported.
/// </summary>
public readonly struct AheadOfTimeReminderTime : IComparable<AheadOfTimeReminderTime>, IEquatable<AheadOfTimeReminderTime>
{
    const string TimeSpanFormat = "HH:mm";

    readonly TimeSpan _timeSpan;

    public int Hours => _timeSpan.Hours;

    public int Minutes => _timeSpan.Minutes;

    public TimeSpan TimeSpan => _timeSpan;

    public AheadOfTimeReminderTime(int hours, int minutes)
    {
        ValidateConstructorArguments(hours, minutes);
        _timeSpan = new(hours, minutes, 0);
    }

    public AheadOfTimeReminderTime(TimeSpan timeSpan)
    {
        if (timeSpan.Seconds != 0 || timeSpan.Milliseconds != 0 || timeSpan.Microseconds != 0 || timeSpan.Nanoseconds != 0)
        {
            ThrowHelper.ThrowArgumentException(nameof(timeSpan), "Seconds and smaller units are not supported.");
        }
        ValidateConstructorArguments(timeSpan.Hours, timeSpan.Minutes);
        _timeSpan = timeSpan;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ValidateConstructorArguments(int hours, int minutes)
    {
        Guard.IsBetween(hours, 0, 24);
        if (hours != 24)
        {
            Guard.IsBetween(minutes, 0, 59);
        }
        else if (minutes != 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(minutes), "The given time exceeds 24:00.");
        }
    }

    public int CompareTo(AheadOfTimeReminderTime other)
    {
        return _timeSpan.CompareTo(other._timeSpan);
    }

    public bool Equals(AheadOfTimeReminderTime other)
    {
        return _timeSpan.Equals(other._timeSpan);
    }

    public override bool Equals(object? obj)
    {
        return obj is AheadOfTimeReminderTime other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _timeSpan.GetHashCode();
    }

    public override string ToString()
    {
        return _timeSpan.ToString(TimeSpanFormat);
    }

    public static bool operator ==(AheadOfTimeReminderTime left, AheadOfTimeReminderTime right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AheadOfTimeReminderTime left, AheadOfTimeReminderTime right)
    {
        return !(left == right);
    }

    public static bool operator <(AheadOfTimeReminderTime left, AheadOfTimeReminderTime right)
    {
        return left._timeSpan < right._timeSpan;
    }

    public static bool operator <=(AheadOfTimeReminderTime left, AheadOfTimeReminderTime right)
    {
        return left._timeSpan <= right._timeSpan;
    }

    public static bool operator >(AheadOfTimeReminderTime left, AheadOfTimeReminderTime right)
    {
        return !(left <= right);
    }

    public static bool operator >=(AheadOfTimeReminderTime left, AheadOfTimeReminderTime right)
    {
        return !(left < right);
    }
}
