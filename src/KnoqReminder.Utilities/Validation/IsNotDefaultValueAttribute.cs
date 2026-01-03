using System.ComponentModel.DataAnnotations;

namespace KnoqReminder.Utilities.Validation;

public sealed class IsNotDefaultValueAttribute<T> : ValidationAttribute
    where T : struct, IEquatable<T>
{
    public override bool IsValid(object? value)
    {
        return value is T typedValue && !typedValue.Equals(default);
    }
}
