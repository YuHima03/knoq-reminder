using System.Runtime.CompilerServices;
using KnoqReminder.Domain.Services.Localization;

namespace KnoqReminder.App.Services.Localization;

sealed class LocalTimeProvider(TimeZoneInfo timeZoneInfo) : ILocalTimeProvider
{
    public DateTimeOffset LocalNow => ToLocalDateTimeOffset(DateTimeOffset.UtcNow);

    public DateOnly LocalToday => DateOnly.FromDateTime(ToLocalDateTime(DateTime.UtcNow).Date);

    public TimeSpan TimeZoneOffset => TimeZoneInfo.BaseUtcOffset;

    public TimeZoneInfo TimeZoneInfo => timeZoneInfo;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime ToLocalDateTime(DateTime utcDateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTimeOffset ToLocalDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToOffset(TimeZoneOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime ToUtcDateTime(DateTime localDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, TimeZoneInfo);
    }
}
