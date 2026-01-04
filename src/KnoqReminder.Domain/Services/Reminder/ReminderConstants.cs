using System.Diagnostics.CodeAnalysis;

namespace KnoqReminder.Domain.Services.Reminder;

public static class ReminderConstants
{
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    public const string EventDateTimeFormat = "MM/dd(ddd) HH:mm";

    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    public const string EventDateTimeFormatTimeOnly = "HH:mm";

    [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)]
    public const string TodayDateOnlyFormat = "MM/dd (ddd)";

    public const int MaxEventNameLength = 256;
    public const int MaxEventHostNameLength = 256;
    public const int MaxEventPlaceNameLength = 1024;
    public const int MaxEventDescriptionLength = 2048;
}
