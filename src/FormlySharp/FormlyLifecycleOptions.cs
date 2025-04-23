namespace FormlySharp;

using System.Text.Json.Serialization;

/// <summary>
/// Configuration options for field lifecycle hooks.
/// These hooks allow executing JavaScript code at specific points in a field's lifecycle.
/// </summary>
public record FormlyLifecycleOptions
{
    /// <summary>
    /// JavaScript function or expression to execute when the field is initialized.
    /// </summary>
    [JsonPropertyName("onInit")]        public string? OnInit { get; init; }
    
    /// <summary>
    /// JavaScript function or expression to execute when the field's inputs change.
    /// </summary>
    [JsonPropertyName("onChanges")]     public string? OnChanges { get; init; }
    
    /// <summary>
    /// JavaScript function or expression to execute after the field's content has been initialized.
    /// </summary>
    [JsonPropertyName("afterContentInit")] public string? AfterContentInit { get; init; }
    
    /// <summary>
    /// JavaScript function or expression to execute after the field's view has been initialized.
    /// </summary>
    [JsonPropertyName("afterViewInit")] public string? AfterViewInit { get; init; }
    
    /// <summary>
    /// JavaScript function or expression to execute when the field is being destroyed.
    /// </summary>
    [JsonPropertyName("onDestroy")]     public string? OnDestroy { get; init; }
}