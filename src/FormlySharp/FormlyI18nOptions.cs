namespace FormlySharp;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Internationalization options for Formly fields.
/// Contains translation keys that can be resolved on the client side.
/// </summary>
public record FormlyI18nOptions
{
    /// <summary>
    /// Translation key for the field label.
    /// </summary>
    [JsonPropertyName("labelKey")] public string? LabelKey { get; init; }

    /// <summary>
    /// Translation key for the field placeholder.
    /// </summary>
    [JsonPropertyName("placeholderKey")] public string? PlaceholderKey { get; init; }

    /// <summary>
    /// Translation key for the field description.
    /// </summary>
    [JsonPropertyName("descriptionKey")] public string? DescriptionKey { get; init; }

    /// <summary>
    /// Translation keys for validation messages.
    /// Keys are validator names, values are translation keys.
    /// </summary>
    [JsonPropertyName("validationMessages")] public Dictionary<string, string>? ValidationMessages { get; init; }

    /// <summary>
    /// The locale code to use for this field (e.g., "en-US", "fr-FR").
    /// This can be used for locale-specific rendering or validation.
    /// </summary>
    [JsonPropertyName("locale")] public string? Locale { get; init; }

    /// <summary>
    /// Additional translation keys for custom properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object> AdditionalTranslations { get; init; } = new Dictionary<string, object>();
}