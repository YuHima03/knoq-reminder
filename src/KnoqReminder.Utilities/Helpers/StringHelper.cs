namespace KnoqReminder.Utilities.Helpers;

static class StringHelper
{
    public static string Truncate(this string s, int maxLength)
    {
        if (s is null || maxLength == 0)
        {
            return string.Empty;
        }
        return s.Length <= maxLength ? s : string.Concat(s.AsSpan(0, maxLength - 1), "…");
    }
}
