using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using KnoqReminder.Domain.Options;

namespace KnoqReminder.App.Configurations;

public class TraqClientConfiguration : ITraqClientOptions
{
    public const string EnvironmentPrefix = "TRAQ_";

    public const string Position = "Traq";

    [ConfigurationKeyName(Position + ":BaseUrl:Api")]
    [NotNull]
    [Required]
    public Uri? ApiBaseUrl { get; set; }

    [ConfigurationKeyName(EnvironmentPrefix + "ACCESS_TOKEN")]
    [NotNull]
    [Required(AllowEmptyStrings = false,
        ErrorMessage = $"The configuration {EnvironmentPrefix}ACCESS_TOKEN is not set.")]
    public string? AccessToken { get; set; }
}
