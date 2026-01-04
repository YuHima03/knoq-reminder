namespace KnoqReminder.Domain.Services.Localization;

public interface ILocalTimeProvider
{
    DateTimeOffset LocalNow { get; }

    DateOnly LocalToday { get; }

    TimeSpan TimeZoneOffset { get; }

    TimeZoneInfo TimeZoneInfo { get; }

    DateTime ToLocalDateTime(DateTime utcDateTime);

    DateTimeOffset ToLocalDateTimeOffset(DateTimeOffset dateTimeOffset);

    DateTime ToUtcDateTime(DateTime localDateTime);
}
