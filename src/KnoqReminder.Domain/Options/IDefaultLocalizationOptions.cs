using System.Globalization;

namespace KnoqReminder.Domain.Options;

public interface IDefaultLocalizationOptions
{
    CultureInfo CultureInfo { get; }

    TimeZoneInfo TimeZoneInfo { get; }
}
