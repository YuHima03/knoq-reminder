using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

public class KnoqClientConfiguration : IKnoqClientOptions
{
    public const string EnvironmentPrefix = "KNOQ_";

    public const string Position = "Knoq";

    [ConfigurationKeyName($"{Position}:BaseUrl:Api")]
    [NotNull]
    [Required]
    public Uri? ApiBaseUrl { get; set; }

    [ConfigurationKeyName($"{Position}:BaseUrl:WebPage")]
    [NotNull]
    [Required]
    public Uri? WebPageBaseUrl { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "USERNAME")]
    [NotNull]
    [Required(AllowEmptyStrings = false,
        ErrorMessage = $"The configuration {EnvironmentPrefix}USERNAME is not set.")]
    public string? Username { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "PASSWORD")]
    [NotNull]
    [Required(AllowEmptyStrings = false,
        ErrorMessage = $"The configuration {EnvironmentPrefix}PASSWORD is not set.")]
    public string? Password { get; set; }
}
