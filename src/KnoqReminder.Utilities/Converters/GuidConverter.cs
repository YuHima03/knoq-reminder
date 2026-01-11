using System.Diagnostics.CodeAnalysis;
using KnoqReminder.Utilities.Collections;

namespace KnoqReminder.Utilities.Converters;

public static class GuidConverter
{
    public static bool TryConvertToGuid([NotNullWhen(false)] this object? value, out Guid guid)
    {
        if (value is Guid g)
        {
            guid = g;
            return true;
        }
        else if (value is IEnumerable<char> chars && chars.TryGetReadOnlySpan(out var charsSpan))
        {
            return Guid.TryParse(charsSpan, out guid);
        }
        else if (value is IEnumerable<byte> bytes && bytes.TryGetReadOnlySpan(out var bytesSpan))
        {
            return Guid.TryParse(bytesSpan, out guid);
        }
        guid = default;
        return false;
    }
}
