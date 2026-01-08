using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

public class TraqClientConfiguration : ITraqClientOptions
{
    public const string Position = "Traq";

    [ConfigurationKeyName("BaseUrl:Api")]
    [NotNull]
    [Required]
    public Uri? ApiBaseUrl { get; set; }

    [ConfigurationKeyName("AccessToken")]
    [NotNull]
    [Required(AllowEmptyStrings = false,
        ErrorMessage = $"The configuration {Position}:AccessToken is not set.")]
    public string? AccessToken { get; set; }
}
