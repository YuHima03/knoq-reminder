using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;

namespace KnoqReminder.Domain.Models;

/// <summary>
/// Represents a specific time of day for daily reminders, within the range 00:00 to 23:59.
/// Seconds and smaller units are not supported.
/// </summary>
public readonly struct DailyReminderTime : IComparable<DailyReminderTime>, IEquatable<DailyReminderTime>
{
    const string TimeOnlyFormat = "HH:mm";

    readonly TimeOnly _timeOnly;

    public int Hours => _timeOnly.Hour;

    public int Minutes => _timeOnly.Minute;

    public DailyReminderTime(int hour, int minute)
    {
        _timeOnly = new TimeOnly(hour, minute);
    }

    public DailyReminderTime(TimeOnly timeOnly)
    {
        if (timeOnly.Second != 0 || timeOnly.Millisecond != 0 || timeOnly.Microsecond != 0 || timeOnly.Nanosecond != 0)
        {
            ThrowHelper.ThrowArgumentException("Seconds and smaller units are not supported.", nameof(timeOnly));
        }
        _timeOnly = timeOnly;
    }

    public int CompareTo(DailyReminderTime other)
    {
        return _timeOnly.CompareTo(other._timeOnly);
    }

    public bool Equals(DailyReminderTime other)
    {
        return _timeOnly.Equals(other._timeOnly);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is DailyReminderTime other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _timeOnly.GetHashCode();
    }

    public override string ToString()
    {
        return _timeOnly.ToString(TimeOnlyFormat);
    }

    public static bool operator ==(DailyReminderTime left, DailyReminderTime right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DailyReminderTime left, DailyReminderTime right)
    {
        return !(left == right);
    }

    public static bool operator <(DailyReminderTime left, DailyReminderTime right)
    {
        return left._timeOnly < right._timeOnly;
    }

    public static bool operator <=(DailyReminderTime left, DailyReminderTime right)
    {
        return left._timeOnly <= right._timeOnly;
    }

    public static bool operator >(DailyReminderTime left, DailyReminderTime right)
    {
        return !(left <= right);
    }

    public static bool operator >=(DailyReminderTime left, DailyReminderTime right)
    {
        return !(left < right);
    }
}
