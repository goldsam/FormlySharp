namespace FormlySharp;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Configuration options for field validation behavior and messages.
/// </summary>
public record FormlyValidationOptions
{
    /// <summary>
    /// Custom validation messages keyed by validator name.
    /// These messages will be displayed when the corresponding validator fails.
    /// </summary>
    [JsonPropertyName("messages")]      public Dictionary<string, string>? Messages { get; init; }
    
    /// <summary>
    /// Controls whether validation messages should be displayed for this field.
    /// </summary>
    [JsonPropertyName("show")]          public bool? Show { get; init; }
}