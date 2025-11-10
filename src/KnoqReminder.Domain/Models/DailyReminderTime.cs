using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;

namespace KnoqReminder.Domain.Models;

/// <summary>
/// Represents a specific time of day for daily reminders, within the range 00:00 to 23:59.
/// Seconds and smaller units are not supported.
/// </summary>
public readonly struct DailyReminderTime : IComparable<DailyReminderTime>, IEquatable<DailyReminderTime>, ISpanFormattable, ISpanParsable<DailyReminderTime>, IUtf8SpanFormattable
{
    const string TimeOnlyFormat = "HH:mm";

    readonly TimeOnly _timeOnly;

    public int Hour => _timeOnly.Hour;

    public int Minute => _timeOnly.Minute;

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

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return _timeOnly.ToString(format, formatProvider);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return _timeOnly.TryFormat(destination, out charsWritten, format, provider);
    }

    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return _timeOnly.TryFormat(utf8Destination, out bytesWritten, format, provider);
    }

    public static DailyReminderTime Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return new(TimeOnly.Parse(s, provider));
    }

    public static DailyReminderTime Parse(string s, IFormatProvider? provider)
    {
        return new(TimeOnly.Parse(s, provider));
    }

    static bool TryCreate(TimeOnly timeOnly, [MaybeNullWhen(false)] out DailyReminderTime result)
    {
        try
        {
            result = new(timeOnly);
            return true;
        }
        catch (ArgumentException)
        {
            result = default;
            return false;
        }
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out DailyReminderTime result)
    {
        result = default;
        return TimeOnly.TryParse(s, provider, out var timeOnly) && TryCreate(timeOnly, out result);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out DailyReminderTime result)
    {
        result = default;
        return TimeOnly.TryParse(s, provider, out var timeOnly) && TryCreate(timeOnly, out result);
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
