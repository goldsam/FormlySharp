namespace FormlySharp;

using System.Text.Json.Serialization;

/// <summary>
/// Configuration options for how and when the model value is updated from field changes.
/// </summary>
public record FormlyModelOptions
{
    /// <summary>
    /// Time in milliseconds to wait after the last keystroke before updating the model.
    /// This helps prevent excessive model updates while typing.
    /// </summary>
    [JsonPropertyName("debounce")]      public int? Debounce { get; init; }
    
    /// <summary>
    /// The event that triggers a model update.
    /// Typical values include "change", "blur", or "submit".
    /// </summary>
    [JsonPropertyName("updateOn")]      public string? UpdateOn { get; init; }
}