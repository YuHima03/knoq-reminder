using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using KnoqReminder.Domain.Options;
using KnoqReminder.Domain.Services.Localization;
using Microsoft.Extensions.Options;

namespace KnoqReminder.App.Services.Localization;

sealed partial class LocalTimeProvider(
    IOptions<IDefaultLocalizationOptions> options
    ) : ILocalTimeProvider
{
    public DateTimeOffset LocalNow => ToLocalDateTimeOffset(DateTimeOffset.UtcNow);

    public DateOnly LocalToday => DateOnly.FromDateTime(ToLocalDateTime(DateTime.UtcNow).Date);

    public TimeSpan TimeZoneOffset => TimeZoneInfo.BaseUtcOffset;

    public TimeZoneInfo TimeZoneInfo { get; } = options.Value.TimeZoneInfo;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime ToLocalDateTime(DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            ThrowHelper.ThrowArgumentException(nameof(utcDateTime), "The provided DateTime must be in UTC.");
        }
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTimeOffset ToLocalDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        var localDateTime = ToLocalDateTime(dateTimeOffset.UtcDateTime);
        return new DateTimeOffset(localDateTime, TimeZoneInfo.BaseUtcOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime ToUtcDateTime(DateTime localDateTime)
    {
        if (localDateTime.Kind == DateTimeKind.Utc)
        {
            return localDateTime;
        }
        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, TimeZoneInfo);
    }
}
