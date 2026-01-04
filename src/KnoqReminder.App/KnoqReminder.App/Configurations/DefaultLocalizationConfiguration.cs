using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

class DefaultLocalizationConfiguration : IDefaultLocalizationOptions
{
    public const string Position = "Localization:Default";

    [ConfigurationKeyName("Culture")]
    public string? CultureName { get; set; }

    CultureInfo IDefaultLocalizationOptions.CultureInfo => TryGetCulture(CultureName, out var ci) ? ci : CultureInfo.InvariantCulture;

    [ConfigurationKeyName("TimeZone")]
    public string? TimeZoneId { get; set; }

    TimeZoneInfo IDefaultLocalizationOptions.TimeZoneInfo => TryGetTimeZone(TimeZoneId, out var tzi) ? tzi : TimeZoneInfo.Utc;

    static bool TryGetCulture(string? name, [MaybeNullWhen(false)] out CultureInfo ci)
    {
        if (string.IsNullOrEmpty(name))
        {
            ci = CultureInfo.InvariantCulture;
            return true;
        }
        try
        {
            ci = CultureInfo.GetCultureInfo(name);
            return true;
        }
        catch (CultureNotFoundException)
        {
            ci = null;
            return false;
        }
    }

    static bool TryGetTimeZone(string? id, [MaybeNullWhen(false)] out TimeZoneInfo tzi)
    {
        if (string.IsNullOrEmpty(id))
        {
            tzi = TimeZoneInfo.Utc;
            return true;
        }
        if (OperatingSystem.IsWindows() && TimeZoneInfo.TryConvertIanaIdToWindowsId(id, out var windowsId))
        {
            id = windowsId;
        }
        return TimeZoneInfo.TryFindSystemTimeZoneById(id, out tzi);
    }
}
