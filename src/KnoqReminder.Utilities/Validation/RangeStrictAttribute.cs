using System.ComponentModel.DataAnnotations;

namespace KnoqReminder.Utilities.Validation;

public class RangeStrictAttribute<T>(string? minimum, string? maximum) : ValidationAttribute
    where T : IParsable<T>, IComparable<T>
{
    public IFormatProvider? FormatProvider { get; set; } = null;

    public bool MaximumIsExclusive { get; set; } = false;

    public bool MinimumIsExclusive { get; set; } = false;

    public override bool IsValid(object? value)
    {
        if (value is not T typedValue)
        {
            return false;
        }
        if (minimum is not null)
        {
            if (!T.TryParse(minimum, FormatProvider, out var min))
            {
                return false;
            }
            var diff = typedValue.CompareTo(min);
            if (diff < 0 || (MinimumIsExclusive && diff == 0))
            {
                return false;
            }
        }
        if (maximum is not null)
        {
            if (!T.TryParse(maximum, FormatProvider, out var max))
            {
                return false;
            }
            var diff = typedValue.CompareTo(max);
            if (diff > 0 || (MaximumIsExclusive && diff == 0))
            {
                return false;
            }
        }
        return true;
    }
}
