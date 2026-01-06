using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

public class KnoqClientConfiguration : IKnoqClientOptions
{
    public const string Position = "Knoq";

    [ConfigurationKeyName("BaseUrl:Api")]
    [NotNull]
    [Required]
    public Uri? ApiBaseUrl { get; set; }

    [ConfigurationKeyName("BaseUrl:WebPage")]
    [NotNull]
    [Required]
    public Uri? WebPageBaseUrl { get; set; }

    [ConfigurationKeyName("Username")]
    [NotNull]
    [Required(AllowEmptyStrings = false,
        ErrorMessage = $"The configuration {Position}:Username is not set.")]
    public string? Username { get; set; }

    [ConfigurationKeyName("Password")]
    [NotNull]
    [Required(AllowEmptyStrings = false,
        ErrorMessage = $"The configuration {Position}:Password is not set.")]
    public string? Password { get; set; }
}
